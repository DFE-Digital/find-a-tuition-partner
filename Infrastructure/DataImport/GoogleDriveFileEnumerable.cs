using System.Collections;
using Application.DataImport;
using Infrastructure.Factories;
using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace Infrastructure.DataImport;

public sealed class GoogleDriveFileEnumerable : IEnumerable<DataFile>, IEnumerator<DataFile>
{
    private readonly string _directoryId;
    private readonly Func<GoogleDriveFileService, GoogleFile, Stream> _downloadFileFn;
    private readonly GoogleDriveFileService _service;
    private readonly List<GoogleFile> _files = new();
    private readonly List<DataFile> _dataFiles = new();
    private int _index = -1;
    private bool _initialized;

    public GoogleDriveFileEnumerable(
        GoogleDriveServiceFactory googleDriveServiceFactory,
        string directoryId,
        Func<GoogleDriveFileService, GoogleFile, Stream> downloadFile)
    {
        _service = googleDriveServiceFactory.GetDriveFiles();
        _directoryId = directoryId;
        _downloadFileFn = downloadFile;
    }

    public bool MoveNext()
    {
        if (!_initialized)
        {
            RetrieveFileList();
            _initialized = true;
        }

        if (_index >= _files.Count - 1) return false;

        _index++;

        Current = GetCurrent();

        return true;
    }

    private void RetrieveFileList()
    {
        var files = _service.FindAllFiles($"parents in '{_directoryId}'");
        _files.AddRange(files);
    }

    private DataFile GetCurrent()
    {
        if (_index >= _files.Count)
        {
            throw new InvalidOperationException($"Current index {_index} is outside valid range");
        }

        if (_index < _dataFiles.Count) return _dataFiles[_index];

        var file = _files[_index];

        var dataFile = new DataFile(file.Name, new Lazy<Stream>(() =>
        {
            return _downloadFileFn(_service, file);
        }));

        return dataFile;
    }

    public IEnumerator<DataFile> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public DataFile Current { get; private set; } = new("");

    object IEnumerator.Current => Current;

    public void Reset() => _index = -1;

    public void Dispose()
    {
        foreach (var dataFile in _dataFiles)
            dataFile.Dispose();
    }
}