using System.Collections;
using Application.DataImport;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using File = Google.Apis.Drive.v3.Data.File;

namespace Infrastructure.DataImport;

public class GoogleDriveDataFileEnumerable : IDataFileEnumerable, IEnumerator<DataFile>
{
    private const string SchoolServicesSharedDriveId = "0ALN4QSSNcSvkUk9PVA";
    private const string TuitionPartnerDataFolderId = "1uda1n7cWHS4goRuDxhJBHoURAlAgh3YB";
    private const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly ILogger<GoogleDriveDataFileEnumerable> _logger;
    private readonly DriveService _service;
    private readonly List<File> _files = new();
    private readonly List<DataFile> _dataFiles = new();
    private int _index = -1;
    private bool _initialized;

    public GoogleDriveDataFileEnumerable(IOptions<GoogleDrive> config, ILogger<GoogleDriveDataFileEnumerable> logger)
    {
        var googleDrive = config.Value;
        _logger = logger;

        GoogleCredential credential;
        if (!string.IsNullOrEmpty(googleDrive.ServiceAccountKey))
        {
            _logger.LogInformation("Loading Google Drive service account key from environment variable");
            try
            {
                credential = GoogleCredential.FromJson(googleDrive.ServiceAccountKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loading Google Drive service account key from environment variable threw exception");
                throw;
            }
        }
        else if (!string.IsNullOrEmpty(googleDrive.ServiceAccountKeyFilePath))
        {
            _logger.LogInformation($"Loading Google Drive service account key from {googleDrive.ServiceAccountKeyFilePath}");
            try
            {
                credential = GoogleCredential.FromFile(googleDrive.ServiceAccountKeyFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Loading Google Drive service account key from {googleDrive.ServiceAccountKeyFilePath} threw exception");
                throw;
            }
        }
        else
        {
            throw new InvalidOperationException("Importing files Google Drive requires either the GoogleDrive:ServiceAccountKey (full key JSON) or GoogleDrive:ServiceAccountKeyFilePath (path to key JSON file) configuration value. GoogleDrive:ServiceAccountKey is expected for deployments and has priority. GoogleDrive:ServiceAccountKeyFilePath is used for local development via 'dotnet user-secrets set \"GoogleDrive:ServiceAccountKeyFilePath\" \"<PATH_TO_KEY_JSON_FILE>\" -p UI'");
        }

        _service = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential.CreateScoped(DriveService.Scope.DriveReadonly)
        });
    }

    public IEnumerator<DataFile> GetEnumerator()
    {
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool MoveNext()
    {
        if (!_initialized)
        {
            RetrieveFiles();
            _initialized = true;
        }

        if (_index >= _files.Count - 1) return false;

        _index++;

        Current = GetCurrent();

        return true;
    }

    private void RetrieveFiles()
    {
        var listRequest = _service.Files.List();

        listRequest.DriveId = SchoolServicesSharedDriveId;
        listRequest.Q = $"parents in '{TuitionPartnerDataFolderId}' and trashed = false";

        // Following must be set when searching within a shared drive
        listRequest.IncludeItemsFromAllDrives = true;
        listRequest.SupportsAllDrives = true;
        listRequest.Corpora = "drive";

        listRequest.OrderBy = "name";
        listRequest.Fields = "nextPageToken, files(id, name, mimeType)";
        listRequest.PageSize = 50;

        FileList fileList;
        var page = 1;
        do
        {
            _logger.LogInformation($"Retrieving page {page} of files from folder with id {TuitionPartnerDataFolderId} in shared Google Drive with id {SchoolServicesSharedDriveId}");
            try
            {
                fileList = listRequest.Execute();
                _files.AddRange(fileList.Files);
                _logger.LogInformation($"Retrieved {fileList.Files.Count} files for this request and {_files.Count} in total");
                listRequest.PageToken = fileList.NextPageToken;
                page++;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Retrieving page {page} of files from folder with id {TuitionPartnerDataFolderId} in shared Google Drive with id {SchoolServicesSharedDriveId} caused exception", ex);
                throw;
            }
        } while (!string.IsNullOrEmpty(fileList.NextPageToken));
    }

    private DataFile GetCurrent()
    {
        if (_index >= _files.Count) throw new InvalidOperationException($"Current index {_index} is outside valid range");

        if (_index < _dataFiles.Count) return _dataFiles[_index];

        var file = _files[_index];

        var dataFile = new DataFile(file.Name, new MemoryStream());

        IDownloadProgress? status;
        if (file.MimeType == ExcelMimeType)
        {
            _logger.LogInformation($"Downloading file {file.Name} with id {file.Id} and mime type {file.MimeType} from Google Drive");
            var request = _service.Files.Get(file.Id);
            status = request.DownloadWithStatus(dataFile.Stream);
        }
        else
        {
            _logger.LogInformation($"Exporting file {file.Name} with id {file.Id} and mime type {file.MimeType} from Google Drive");
            var exportRequest = _service.Files.Export(file.Id, ExcelMimeType);
            status = exportRequest.DownloadWithStatus(dataFile.Stream);
        }

        if (status.Status != DownloadStatus.Completed)
        {
            throw new Exception($"Could not download file {file.Name} with id {file.Id} and mime type {file.MimeType} from Google Drive. Status {status.Status} bytes downloaded {status.BytesDownloaded} exception {status.Exception}");
        }

        _logger.LogInformation($"Successfully downloaded file {file.Name} with id {file.Id} and mime type {file.MimeType} from Google Drive. Status {status.Status} bytes downloaded {status.BytesDownloaded} exception {status.Exception}");

        _dataFiles.Add(dataFile);

        return dataFile;
    }

    public void Reset()
    {
        _index = -1;
    }

    public DataFile Current { get; private set; } = new("", new MemoryStream());

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        foreach (var dataFile in _dataFiles)
        {
            dataFile.Dispose();
        }
    }
}