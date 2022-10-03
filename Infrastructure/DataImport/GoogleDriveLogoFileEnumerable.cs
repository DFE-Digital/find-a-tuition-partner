using System.Collections;
using Application.DataImport;
using Infrastructure.Factories;
using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace Infrastructure.DataImport;

public sealed class GoogleDriveLogoFileEnumerable : ILogoFileEnumerable, IEnumerator<DataFile>
{
    private readonly GoogleDriveFileEnumerable _enum;

    public GoogleDriveLogoFileEnumerable(GoogleDriveServiceFactory googleDriveServiceFactory)
    {
        _enum = new GoogleDriveFileEnumerable(googleDriveServiceFactory, "Logos", DownloadFile);
    }

    private static Stream DownloadFile(GoogleDriveFileService service, GoogleFile file)
        => service.Download(file.Id, file.Name);

    public IEnumerator<DataFile> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool MoveNext() => _enum.MoveNext();

    public void Reset() => _enum.Reset();

    public DataFile Current => _enum.Current;

    object IEnumerator.Current => Current;

    public void Dispose() => _enum.Dispose();
}
