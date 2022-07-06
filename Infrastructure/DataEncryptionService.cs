using Infrastructure.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public class DataEncryptionService : IHostedService
{
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
            _logger.LogError("Encrypting files requires the following two arguments: --DataEncryption:SourceDirectory and --DataEncryption:Key e.g. --DataEncryption:SourceDirectory=\"C:\\Quality Assured\\\" --DataEncryption:Key=\"2c56c54e-10fd-4f91-a343-0218d9372894\"");

            _host.StopApplication();

            return;
        }

        _logger.LogWarning($"Encrypting files from {_config.SourceDirectory} and replacing all existing files in the /Infrastructure/Data directory");

        _host.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}