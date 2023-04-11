using System.Collections;
using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.DataImport;

namespace Infrastructure.DataImport;

public sealed class OneDriveFileEnumerable : IEnumerable<DataFile>, IEnumerator<DataFile>
{
    private readonly string _folderId;
    private readonly string[]? _fileExtensions;
    private readonly IOneDriveApiClient _client;
    private readonly List<DriveItem> _files = new();
    private readonly List<DataFile> _dataFiles = new();
    private int _index = -1;
    private bool _initialized;

    public OneDriveFileEnumerable(
        IOneDriveApiClient client,
        string folderId, string[]? fileExtensions)
    {
        _client = client;
        _folderId = folderId;
        _fileExtensions = fileExtensions;
    }

    public bool MoveNext()
    {
        if (!_initialized)
        {
            RetrieveFileListAsync().GetAwaiter().GetResult();
            _initialized = true;
        }

        if (_index >= _files.Count - 1) return false;

        _index++;

        Current = GetCurrent();

        return true;
    }

    private async Task RetrieveFileListAsync()
    {
        var files = await _client.GetFilesInFolder(_folderId, _fileExtensions);
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
            _client.DownloadFile(file.Id, file.Name).GetAwaiter().GetResult()));

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