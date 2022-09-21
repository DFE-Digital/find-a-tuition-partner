using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Factories;

public class GoogleDriveServiceFactory
{
    private readonly GoogleDrive _config;
    private readonly ILogger<GoogleDriveServiceFactory> _logger;

    public GoogleDriveServiceFactory(IOptions<GoogleDrive> config, ILogger<GoogleDriveServiceFactory> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    public DriveService GetDriveService()
    {
        GoogleCredential credential;
        if (!string.IsNullOrEmpty(_config.ServiceAccountKey))
        {
            _logger.LogInformation("Loading Google Drive service account key from environment variable");
            try
            {
                credential = GoogleCredential.FromJson(_config.ServiceAccountKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loading Google Drive service account key from environment variable threw exception");
                throw;
            }
        }
        else if (!string.IsNullOrEmpty(_config.ServiceAccountKeyFilePath))
        {
            _logger.LogInformation($"Loading Google Drive service account key from {_config.ServiceAccountKeyFilePath}");
            try
            {
                credential = GoogleCredential.FromFile(_config.ServiceAccountKeyFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Loading Google Drive service account key from {_config.ServiceAccountKeyFilePath} threw exception");
                throw;
            }
        }
        else
        {
            throw new InvalidOperationException("Importing files Google Drive requires either the GoogleDrive:ServiceAccountKey (full key JSON) or GoogleDrive:ServiceAccountKeyFilePath (path to key JSON file) configuration value. GoogleDrive:ServiceAccountKey is expected for deployments and has priority. GoogleDrive:ServiceAccountKeyFilePath is used for local development via 'dotnet user-secrets set \"GoogleDrive:ServiceAccountKeyFilePath\" \"<PATH_TO_KEY_JSON_FILE>\" -p UI'");
        }

        var service = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential.CreateScoped(DriveService.Scope.DriveReadonly)
        });

        return service;
    }
}