using System.Collections;
using Application.Common.Interfaces;
using Application.DataImport;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataImport;

public class AzureBlobStorageDataFileEnumerable : IDataFileEnumerable, IEnumerator<DataFile>
{
    private readonly AzureBlobStorageFileEnumerable _enum;

    public AzureBlobStorageDataFileEnumerable(IOptions<AzureBlobStorageSettings> config, IAzureBlobStorageService service)
    {
        _enum = new AzureBlobStorageFileEnumerable(service, config.Value.TuitionPartnerDataFolderName);
    }

    public IEnumerator<DataFile> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool MoveNext() => _enum.MoveNext();

    public void Reset() => _enum.Reset();

    public DataFile Current => _enum.Current;

    object IEnumerator.Current => Current;

    public void Dispose() => _enum.Dispose();
}