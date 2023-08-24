using Application.Common.Interfaces;
using Application.Constants;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;

namespace Functions;

public class GenerateUserDelegationSasToken : IGenerateUserDelegationSasTokenAsync
{
    private readonly BlobStorageEnquiriesDataSettings _config;

    public GenerateUserDelegationSasToken(IOptions<BlobStorageEnquiriesDataSettings> config)
    {
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
            .GetUserDelegationKeyAsync(DateTimeOffset.UtcNow.AddSeconds(-300), // sets the start time to be 300 seconds earlier, allowing for possible clock skew between the client and server.
                DateTimeOffset.UtcNow.AddHours(IntegerConstants.AzureBlobStorageServiceTokenExpiryInHour));

        // Create a BlobSasBuilder object
        var expiry = DateTimeOffset.UtcNow.AddHours(IntegerConstants.AzureBlobStorageServiceTokenExpiryInHour);
        var sasBuilder = new BlobSasBuilder(BlobContainerSasPermissions.Read | BlobContainerSasPermissions.List
                                                                             | BlobContainerSasPermissions.Write, expiry)
        {
            BlobContainerName = _config.ContainerName,
            Resource = "c", // container
            StartsOn = DateTimeOffset.UtcNow.AddSeconds(-300) // sets the start time of the SAS token to be 300 seconds earlier, allowing for possible clock skew between the client and server.
        };

        // Generate the SAS token using the user delegation key
        var sasToken = sasBuilder.ToSasQueryParameters(userDelegationKey.Value, blobServiceClient.AccountName).ToString();

        return sasToken;
    }
}