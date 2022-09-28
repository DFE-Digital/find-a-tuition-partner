using System.Collections;
using Application.DataImport;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Infrastructure.Configuration;
using Infrastructure.Factories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using File = Google.Apis.Drive.v3.Data.File;

namespace Infrastructure.DataImport;

public class GoogleDriveDataFileEnumerable : IDataFileEnumerable, IEnumerator<DataFile>
{
    private const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly GoogleDrive _config;
    private readonly DriveService _service;
    private readonly ILogger<GoogleDriveDataFileEnumerable> _logger;
    private readonly List<File> _files = new();
    private readonly List<DataFile> _dataFiles = new();
    private int _index = -1;
    private bool _initialized;

    public GoogleDriveDataFileEnumerable(IOptions<GoogleDrive> config, GoogleDriveServiceFactory googleDriveServiceFactory, ILogger<GoogleDriveDataFileEnumerable> logger)
    {
        _config = config.Value;
        _service = googleDriveServiceFactory.GetDriveService();
        _logger = logger;
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

        listRequest.DriveId = _config.SharedDriveId;
        listRequest.Q = $"parents in '{_config.TuitionPartnerDataFolderId}' and trashed = false";

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
            _logger.LogInformation($"Retrieving page {page} of files from folder with id {_config.TuitionPartnerDataFolderId} in shared Google Drive with id {_config.SharedDriveId}");
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
                _logger.LogError($"Retrieving page {page} of files from folder with id {_config.TuitionPartnerDataFolderId} in shared Google Drive with id {_config.SharedDriveId} caused exception", ex);
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

        _logger.LogInformation($"Successfully downloaded file {file.Name} with id {file.Id} and mime type {file.MimeType} from Google Drive. Status {status.Status} bytes downloaded {status.BytesDownloaded}{(status.Exception == null ? "" : $" exception {status.Exception}")}");

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

public sealed class GoogleDriveLogoFileEnumerable : ILogoFileEnumerable, IEnumerator<DataFile2>
{
    private const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private const string SvgMimeType = "image/svg+xml.";

    private readonly GoogleDrive _config;
    private readonly DriveService _service;
    private readonly ILogger<GoogleDriveDataFileEnumerable> _logger;
    private readonly List<File> _files = new();
    private readonly List<DataFile2> _dataFiles = new();
    private int _index = -1;
    private bool _initialized;

    public GoogleDriveLogoFileEnumerable(IOptions<GoogleDrive> config, GoogleDriveServiceFactory googleDriveServiceFactory, ILogger<GoogleDriveDataFileEnumerable> logger)
    {
        _config = config.Value;
        _service = googleDriveServiceFactory.GetDriveService();
        _logger = logger;
    }

    public IEnumerator<DataFile2> GetEnumerator()
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

        listRequest.DriveId = _config.SharedDriveId;
        listRequest.Q = $"name = 'Logos'";

        // Following must be set when searching within a shared drive
        listRequest.IncludeItemsFromAllDrives = true;
        listRequest.SupportsAllDrives = true;
        listRequest.Corpora = "drive";

        listRequest.OrderBy = "name";
        listRequest.Fields = "nextPageToken, files(id, name, mimeType)";
        listRequest.PageSize = 50;

        var logoList = listRequest.Execute();
        var logoDir = logoList.Files.First();
        listRequest.Q = $"parents in '{logoDir.Id}'";

        FileList fileList;
        var page = 1;
        do
        {
            _logger.LogInformation("Retrieving page {page} of files from folder with id {TuitionPartnerDataFolderId} in shared Google Drive with id {SharedDriveId}",
                page, _config.TuitionPartnerDataFolderId, _config.SharedDriveId);
            try
            {
                fileList = listRequest.Execute();
                _files.AddRange(fileList.Files);
                _logger.LogInformation("Retrieved {PageCount} files for this request and {TotalCount} in total",
                    fileList.Files.Count, _files.Count);
                listRequest.PageToken = fileList.NextPageToken;
                page++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Retrieving page {page} of files from folder with id {TuitionPartnerDataFolderId} in shared Google Drive with id {SharedDriveId} caused exception",
                    page, _config.TuitionPartnerDataFolderId, _config.SharedDriveId);
                throw;
            }
        } while (!string.IsNullOrEmpty(fileList.NextPageToken));
    }

    private DataFile2 GetCurrent()
    {
        if (_index >= _files.Count) throw new InvalidOperationException($"Current index {_index} is outside valid range");

        if (_index < _dataFiles.Count) return _dataFiles[_index];

        var file = _files[_index];

        var dataFile = new DataFile2(file.Name, new Lazy<Stream>(() =>
        {
            var df = new MemoryStream();
            _logger.LogInformation($"Downloading file {file.Name} with id {file.Id} and mime type {file.MimeType} from Google Drive");
            var request = _service.Files.Get(file.Id);
            var status = request.DownloadWithStatus(df);

            if (status.Status != DownloadStatus.Completed)
            {
                throw new Exception($"Could not download file {file.Name} with id {file.Id} and mime type {file.MimeType} from Google Drive. Status {status.Status} bytes downloaded {status.BytesDownloaded} exception {status.Exception}");
            }

            _logger.LogInformation($"Successfully downloaded file {file.Name} with id {file.Id} and mime type {file.MimeType} from Google Drive. Status {status.Status} bytes downloaded {status.BytesDownloaded}{(status.Exception == null ? "" : $" exception {status.Exception}")}");
            return df;
        }));

        //_dataFiles.Add(dataFile);

        return dataFile;
    }

    public void Reset()
    {
        _index = -1;
    }

    public DataFile2 Current { get; private set; } = new("", new Lazy<Stream>());

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        foreach (var dataFile in _dataFiles)
        {
            dataFile.Dispose();
        }
    }
}