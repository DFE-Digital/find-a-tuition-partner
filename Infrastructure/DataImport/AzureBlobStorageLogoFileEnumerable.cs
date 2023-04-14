using System.Collections;
using Application.Common.Interfaces;
using Application.DataImport;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataImport;

public class AzureBlobStorageLogoFileEnumerable : ILogoFileEnumerable, IEnumerator<DataFile>
{
    private readonly AzureBlobStorageFileEnumerable _enum;

    public AzureBlobStorageLogoFileEnumerable(IOptions<AzureBlobStorageSettings> config, IAzureBlobStorageService service)
    {
        _enum = new AzureBlobStorageFileEnumerable(service, config.Value.TuitionPartnerLogosFolderName);
    }

    public IEnumerator<DataFile> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool MoveNext() => _enum.MoveNext();

    public void Reset() => _enum.Reset();

    public DataFile Current => _enum.Current;

    object IEnumerator.Current => Current;

    public void Dispose() => _enum.Dispose();
}