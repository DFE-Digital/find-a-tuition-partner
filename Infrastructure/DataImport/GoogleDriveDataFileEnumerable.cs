﻿using System.Collections;
using Application.DataImport;
using Infrastructure.Factories;
using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace Infrastructure.DataImport;

public sealed class GoogleDriveDataFileEnumerable : IDataFileEnumerable, IEnumerator<DataFile>
{
    private const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private readonly GoogleDriveFileEnumerable _enum;

    public GoogleDriveDataFileEnumerable(GoogleDriveServiceFactory googleDriveServiceFactory)
    {
        _enum = new GoogleDriveFileEnumerable(googleDriveServiceFactory, "FINAL Data Responses", DownloadFile);
    }

    private static Stream DownloadFile(GoogleDriveFileService service, GoogleFile file)
        => file.MimeType == ExcelMimeType
            ? service.Download(file.Id, file.Name)
            : service.Export(file.Id, file.Name, ExcelMimeType);

    public IEnumerator<DataFile> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool MoveNext() => _enum.MoveNext();

    public void Reset() => _enum.Reset();

    public DataFile Current => _enum.Current;

    object IEnumerator.Current => Current;

    public void Dispose() => _enum.Dispose();
}