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
            _logger.LogError("Encrypting files requires the following two arguments: --DataEncryption:SourceDirectory and --DataEncryption:Key e.g. --DataEncryption:SourceDirectory=\"C:\\Quality Assured\" --DataEncryption:Key=\"2c56c54e-10fd-4f91-a343-0218d9372894\"");

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

        _logger.LogWarning($"Encrypting files from {source} and replacing all existing files in the {destination} directory");

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