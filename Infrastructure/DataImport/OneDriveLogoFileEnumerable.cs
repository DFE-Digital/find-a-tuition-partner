using System.Collections;
using Application.Common.Interfaces;
using Application.DataImport;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataImport;

public class OneDriveLogoFileEnumerable : ILogoFileEnumerable, IEnumerator<DataFile>
{
    private readonly OneDriveFileEnumerable _enum;

    public OneDriveLogoFileEnumerable(IOptions<OneDriveSettings> config, IOneDriveApiClient client)
    {
        _enum = new OneDriveFileEnumerable(client, config.Value.TuitionPartnerLogosFolderId, new[]
        {
            ".png", ".svg"
        });
    }

    public IEnumerator<DataFile> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool MoveNext() => _enum.MoveNext();

    public void Reset() => _enum.Reset();

    public DataFile Current => _enum.Current;

    object IEnumerator.Current => Current;

    public void Dispose() => _enum.Dispose();
}