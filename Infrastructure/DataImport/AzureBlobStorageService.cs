using Application.Common.Interfaces;
using Application.Constants;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using BlobItem = Application.Common.DTO.AzureBlobStorage.BlobItem;

namespace Infrastructure.DataImport;

public class AzureBlobStorageService : IAzureBlobStorageService, IGetAccessToken
{
    private const string resource = "https://storage.azure.com/";
    private readonly ILogger<AzureBlobStorageService> _logger;
    private readonly AzureBlobStorageSettings _config;

    public AzureBlobStorageService(ILogger<AzureBlobStorageService> logger,
        IOptions<AzureBlobStorageSettings> config)
    {
        _logger = logger;
        _config = config.Value;
    }

    public async Task<string> GetAccessTokenAsync(string clientId, string clientSecret,
        string tenantId, string resource)
    {
        // See: https://github.com/AzureAD/microsoft-authentication-library-for-dotnet
        var app = ConfidentialClientApplicationBuilder
            .Create(clientId)
            .WithTenantId(tenantId)
            .WithClientSecret(clientSecret)
            .Build();

        var scopes = new[] { $"{resource}/.default" };

        try
        {
            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return result.AccessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to get the access token.");
            throw;
        }
    }

    public async Task<string> GenerateUserDelegationSasTokenAsync()
    {
        var accessToken = await GetAccessTokenAsync(_config.ClientId, _config.ClientSecret, _config.TenantId,
            resource);
        // Create a BlobServiceClient using the account name, account key, and custom token credential
        var blobServiceClient = new BlobServiceClient(new Uri($"https://{_config.StorageAccountName}.blob.core.windows.net"),
            new CustomTokenCredential(accessToken));

        // Get the user delegation key using the custom token credential
        var userDelegationKey = await blobServiceClient
            .GetUserDelegationKeyAsync(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow
                .AddHours(IntegerConstants.AzureBlobStorageServiceTokenExpiryInHour));

        // Create a BlobSasBuilder object
        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = _config.ContainerName,
            Resource = "c", // container
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(IntegerConstants.AzureBlobStorageServiceTokenExpiryInHour)
        };

        // Set the permissions for the SAS token
        sasBuilder.SetPermissions(
            BlobContainerSasPermissions.Read |
            BlobContainerSasPermissions.List
        );

        // Generate the SAS token using the user delegation key
        var sasToken = sasBuilder.ToSasQueryParameters(userDelegationKey.Value, blobServiceClient.AccountName).ToString();

        return sasToken;
    }

    public async Task<List<BlobItem>> GetBlobsFromFoldersAsync(string folderName)
    {
        var blobItems = new List<BlobItem>();

        try
        {
            var sasToken = await GenerateUserDelegationSasTokenAsync();

            // Create a BlobServiceClient with the SAS token
            var blobServiceClient =
                new BlobServiceClient(new Uri($"https://{_config.StorageAccountName}.blob.core.windows.net?{sasToken}"));

            // Get a reference to the container
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_config.ContainerName);

            var blobs = blobContainerClient.GetBlobs(BlobTraits.None,
                BlobStates.None, folderName);

            blobItems.AddRange(blobs.Select(blob => new BlobItem()
            {
                Name = blob.Name,
                Metadata = blob.Metadata,
                Deleted = blob.Deleted
            }));

            return blobItems.Where(x => !x.Deleted).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to get the blob items. Folder name: {folderName}.", folderName);
            throw;
        }
    }

    public async Task<Stream> DownloadFileAsync(string sasToken, string blobName)
    {
        try
        {
            // Create a BlobServiceClient with the SAS token
            var blobServiceClient =
                new BlobServiceClient(new Uri($"https://{_config.StorageAccountName}.blob.core.windows.net?{sasToken}"));

            // Get a reference to the container
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_config.ContainerName);

            // Get a reference to the blob
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            // Download the blob
            var downloadInfo = await blobClient.DownloadAsync();

            // Get the blob's content
            var stream = downloadInfo.Value.Content;

            return stream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to download the file {filename}.", blobName);
            throw;
        }
    }
}