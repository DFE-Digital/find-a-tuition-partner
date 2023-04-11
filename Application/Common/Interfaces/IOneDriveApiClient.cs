using Application.Common.DTO;

namespace Application.Common.Interfaces;

public interface IOneDriveApiClient
{
    Task<List<DriveItem>> GetFilesInFolder(string folderId, string[]? fileExtensions = null);

    Task<Stream> DownloadFile(string fileId, string fileName);
}