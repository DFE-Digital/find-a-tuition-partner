using System.Security.Cryptography;
using System.Text;
using Application.Extensions;
using Infrastructure.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public class DataEncryptionService : IHostedService
{
    private const string DestinationDirectory = "Data";

    private readonly IHostApplicationLifetime _host;
    private readonly DataEncryption _config;
    private readonly ILogger<DataEncryptionService> _logger;

    public DataEncryptionService(IHostApplicationLifetime host, IOptions<DataEncryption> config, ILogger<DataEncryptionService> logger)
    {
        _host = host;
        _config = config.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_config.SourceDirectory) || string.IsNullOrEmpty(_config.Key))
        {
            _logger.LogError("Encrypting files requires the following two arguments: --DataEncryption:SourceDirectory and --DataEncryption:Key e.g. --DataEncryption:SourceDirectory=\"C:\\Quality Assured\" --DataEncryption:Key=\"I0YRt6YZrMvdTSN107O1R5b4lS16Gz7wBMMruEhqAJc=\". These can also be environment variables");

            _host.StopApplication();

            return;
        }

        var source = new DirectoryInfo(_config.SourceDirectory);
        if (!source.Exists)
        {
            _logger.LogError($"Source directory {source} does not exist");

            _host.StopApplication();

            return;
        }

        var destination = GetDestinationDirectoryInfo();
        var keyBytes = Convert.FromBase64String(_config.Key);

        _logger.LogWarning($"Encrypting files from {source} and replacing all existing files in the {destination} directory");

        foreach (var fileInfo in destination.GetFiles().Where(x => x.Name != "README.md"))
        {
            _logger.LogWarning($"Deleting existing file {fileInfo}");
            fileInfo.Delete();
        }

        foreach (var fileInfo in source.GetFiles("*.xlsx"))
        {
            using var crypto = Aes.Create();
            using var cryptoTransform = crypto.CreateEncryptor(keyBytes, crypto.IV);

            var base64Filename = fileInfo.Name.ToBase64Filename();
            var base64Iv = crypto.IV.ToBase64Filename();
            var destinationFilename = $"{base64Filename}_{base64Iv}";
            var destinationFilePath = Path.Combine(destination.FullName, destinationFilename);

            _logger.LogInformation($"Encrypting {fileInfo} and copying to {destinationFilePath}");
            
            await using var sourceStream = fileInfo.OpenRead();
            await using var destinationStream = File.Create(destinationFilePath);
            await using var cryptoStream = new CryptoStream(destinationStream, cryptoTransform, CryptoStreamMode.Write);
            await sourceStream.CopyToAsync(destinationStream, cancellationToken);

            _logger.LogInformation("File encryption successful");
        }

        _logger.LogInformation($"All files encrypted successfully using key {_config.Key}");

        _host.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private DirectoryInfo GetDestinationDirectoryInfo()
    {
        var assembly = typeof(AssemblyReference).Assembly;
        var projectDirectory = assembly.GetName().Name;
        if (projectDirectory == null)
        {
            throw new Exception($"Assembly {assembly} name was null");
        }

        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

        var destination = new DirectoryInfo(Path.Combine(currentDirectory.FullName, projectDirectory, DestinationDirectory));
        while (!destination.Exists)
        {
            currentDirectory = currentDirectory.Parent;
            if (currentDirectory == null) break;

            destination = new DirectoryInfo(Path.Combine(currentDirectory.FullName, projectDirectory, DestinationDirectory));
        }

        if (destination.Exists)
        {
            _logger.LogInformation($"Found destination directory {destination}");
        }
        else
        {
            throw new Exception("Could not find destination directory");
        }

        return destination;
    }
}