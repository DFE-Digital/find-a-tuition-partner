using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Infrastructure.Configuration;
using Infrastructure.DataImport;
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

        if (string.IsNullOrWhiteSpace(_config.TuitionPartnerDataFolderId))
        {
            _logger.LogError("GoogleDrive:TuitionPartnerDataFolderId environment variable not set. Tuition Partner data import will fail. Please check dotnet user-secrets if running on a local development environment or the GOOGLE_DRIVE_TUITION_PARTNER_DATA_FOLDER_ID GitHub Action secret");
        }

        if (string.IsNullOrWhiteSpace(_config.TuitionPartnerLogosFolderId))
        {
            _logger.LogError("GoogleDrive:TuitionPartnerLogosFolderId environment variable not set. Tuition Partner logos import will fail. Please check dotnet user-secrets if running on a local development environment or the GOOGLE_DRIVE_TUITION_PARTNER_LOGO_FOLDER_ID GitHub Action secret");
        }
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

    public GoogleDriveFileService GetDriveFiles()
    {
        return new GoogleDriveFileService(GetDriveService(), _config.SharedDriveId, _logger);
    }
}