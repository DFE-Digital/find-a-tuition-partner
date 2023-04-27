using Application.Common.Interfaces;
using Application.Constants;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BlobItem = Application.Common.DTO.AzureBlobStorage.BlobItem;

namespace Infrastructure.DataImport;

public class AzureBlobStorageService : IAzureBlobStorageService
{
    private readonly ILogger<AzureBlobStorageService> _logger;
    private readonly AzureBlobStorageSettings _config;

    public AzureBlobStorageService(ILogger<AzureBlobStorageService> logger,
        IOptions<AzureBlobStorageSettings> config)
    {
        _logger = logger;
        _config = config.Value;
    }

    public async Task<string> GenerateUserDelegationSasTokenAsync()
    {
        var credential = new ClientSecretCredential(
            _config.TenantId,
            _config.ClientId,
            _config.ClientSecret
        );

        // Create a BlobServiceClient using the account name, account key, and custom token credential
        var blobServiceClient = new BlobServiceClient(new Uri($"https://{_config.AccountName}.blob.core.windows.net"),
            credential);

        // Get the user delegation key using the custom token credential
        var userDelegationKey = await blobServiceClient
            .GetUserDelegationKeyAsync(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow
                .AddHours(IntegerConstants.AzureBlobStorageServiceTokenExpiryInHour));

        // Create a BlobSasBuilder object
        var expiry = DateTimeOffset.UtcNow.AddHours(IntegerConstants.AzureBlobStorageServiceTokenExpiryInHour);
        var sasBuilder = new BlobSasBuilder(BlobContainerSasPermissions.Read | BlobContainerSasPermissions.List, expiry)
        {
            BlobContainerName = _config.ContainerName,
            Resource = "c", // container
            StartsOn = DateTimeOffset.UtcNow.AddSeconds(-60) // sets the start time of the SAS token to be 60 seconds earlier, allowing for possible clock skew between the client and server.
        };

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
                new BlobServiceClient(new Uri($"https://{_config.AccountName}.blob.core.windows.net?{sasToken}"));

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
                new BlobServiceClient(new Uri($"https://{_config.AccountName}.blob.core.windows.net?{sasToken}"));

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