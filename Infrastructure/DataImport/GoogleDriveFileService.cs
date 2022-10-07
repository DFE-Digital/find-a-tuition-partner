using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Microsoft.Extensions.Logging;
using File = Google.Apis.Drive.v3.Data.File;

namespace Infrastructure.DataImport;

public class GoogleDriveFileService
{
    private readonly DriveService _service;
    private readonly string _driveId;
    private readonly ILogger _logger;

    public GoogleDriveFileService(DriveService service, string googleDriveId, ILogger logger)
        => (_service, _driveId, _logger) = (service, googleDriveId, logger);

    public List<File> FindAllDirectories(string search)
        => FindAllEntities($"{search} and mimeType = 'application/vnd.google-apps.folder'");

    public List<File> FindAllFiles(string search)
        => FindAllEntities($"{search} and mimeType != 'application/vnd.google-apps.folder'");

    private List<File> FindAllEntities(string search)
    {
        var listRequest = _service.Files.List();

        listRequest.DriveId = _driveId;
        listRequest.Q = $"{search} and trashed = false";

        // Following must be set when searching within a shared drive
        listRequest.IncludeItemsFromAllDrives = true;
        listRequest.SupportsAllDrives = true;
        listRequest.Corpora = "drive";

        listRequest.OrderBy = "name";
        listRequest.Fields = "nextPageToken, files(id, name, mimeType)";
        listRequest.PageSize = 50;

        FileList fileList;
        List<File> _files = new List<File>();
        var page = 1;
        do
        {
            _logger.LogInformation("Retrieving page {page} of files from search {Query} in shared Google Drive with id {SharedDriveId}",
                page, search, _driveId);
            try
            {
                fileList = listRequest.Execute();
                _files.AddRange(fileList.Files);
                _logger.LogInformation("Retrieved {Count} files for this request and {TotalCount} in total",
                    fileList.Files.Count, _files.Count);
                listRequest.PageToken = fileList.NextPageToken;
                page++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Retrieving page {page} of files from search {Query} in shared Google Drive with id {SharedDriveId} caused exception",
                    page, search, _driveId);
                throw;
            }
        } while (!string.IsNullOrEmpty(fileList.NextPageToken));

        return _files;
    }

    internal Stream Download(string fileId, string fileName)
    {
        _logger.LogInformation("Downloading file {FileName} with id {FileId} from Google Drive", fileName, fileId);

        var stream = new MemoryStream();
        var request = _service.Files.Get(fileId);
        var status = request.DownloadWithStatus(stream);

        if (status.Status == DownloadStatus.Completed) return stream;

        throw new Exception(
            $"Could not download file {fileName} with id {fileId} from Google Drive. Status {status.Status}", status.Exception);
    }

    internal Stream Export(string fileId, string fileName, string mimeType)
    {
        _logger.LogInformation("Exporting file {FileName} with id {FileId} and mime type {MimeType} from Google Drive"
            , fileName, fileId, mimeType);

        var stream = new MemoryStream();
        var exportRequest = _service.Files.Export(fileId, mimeType);
        var status = exportRequest.DownloadWithStatus(stream);

        if (status.Status == DownloadStatus.Completed) return stream;

        throw new Exception(
            $"Could not export file {fileName} with id {fileId} as {mimeType} from Google Drive. Status {status.Status}", status.Exception);
    }
}