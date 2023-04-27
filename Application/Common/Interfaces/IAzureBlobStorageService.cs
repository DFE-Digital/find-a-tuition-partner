using Application.Common.DTO.AzureBlobStorage;

namespace Application.Common.Interfaces;

public interface IAzureBlobStorageService
{
    Task<string> GenerateUserDelegationSasTokenAsync();

    Task<List<BlobItem>> GetBlobsFromFoldersAsync(string folderName);

    Task<Stream> DownloadFileAsync(string sasToken, string blobName);
}