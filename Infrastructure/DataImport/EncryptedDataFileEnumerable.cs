using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Application.DataImport;
using Application.Extensions;
using Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataImport;

public class EncryptedDataFileEnumerable : IDataFileEnumerable, IEnumerator<DataFile>
{
    private readonly ILogger<EncryptedDataFileEnumerable> _logger;
    private readonly byte[] _keyBytes;
    private readonly Assembly _assembly;
    private readonly IList<string> _resourceNames;
    private readonly DataFile?[] _dataFiles;
    private int _index = -1;

    public EncryptedDataFileEnumerable(IOptions<DataEncryption> config, ILogger<EncryptedDataFileEnumerable> logger)
    {
        _logger = logger;

        var dataEncryption = config.Value;
        if (string.IsNullOrEmpty(dataEncryption.Key))
        {
            throw new InvalidOperationException("Importing encrypted files requires the --DataEncryption:Key argument e.g. --DataEncryption:Key \"I0YRt6YZrMvdTSN107O1R5b4lS16Gz7wBMMruEhqAJc=\". These can also be environment variables");
        }
        _keyBytes = Convert.FromBase64String(dataEncryption.Key);

        _assembly = typeof(AssemblyReference).Assembly;
        _resourceNames = _assembly.GetManifestResourceNames().Where(i => !i.EndsWith("README.md")).ToList();
        _dataFiles = new DataFile?[_resourceNames.Count];
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
        if (_index >= _resourceNames.Count - 1) return false;

        _index++;

        Current = GetCurrent();

        return true;
    }

    private DataFile GetCurrent()
    {
        if (_index >= _resourceNames.Count) throw new InvalidOperationException($"Current index {_index} is outside valid range");

        var dataFile = _dataFiles[_index];
        if (dataFile != null) return dataFile;

        var resourceName = _resourceNames[_index];
        var base64Filename = resourceName.Substring(resourceName.LastIndexOf('.') + 1).FromBase64Filename();
        var originalFilename = Encoding.UTF8.GetString(Convert.FromBase64String(base64Filename));

        using var resourceStream = _assembly.GetManifestResourceStream(resourceName);
        if (resourceStream == null)
        {
            throw new InvalidOperationException($"Failed to open manifest resource stream {resourceName}");
        }

        _logger.LogInformation($"Attempting to decrypt encrypted Tuition Partner spreadsheet {resourceName} originally from file {originalFilename}");
        try
        {
            var stream = new MemoryStream();
            using var crypto = Aes.Create();
            var iv = new byte[crypto.IV.Length];
            resourceStream.Read(iv, 0, iv.Length);
            var cryptoTransform = crypto.CreateDecryptor(_keyBytes, iv);
            using var cryptoStream = new CryptoStream(resourceStream, cryptoTransform, CryptoStreamMode.Read);
            cryptoStream.CopyTo(stream);
            dataFile = new DataFile(originalFilename, new Lazy<Stream>(stream));
            _dataFiles[_index] = dataFile;
            return dataFile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception thrown when decrypting encrypted Tuition Partner spreadsheet {resourceName} originally from file {originalFilename}");
            throw;
        }
    }

    public void Reset()
    {
        _index = -1;
    }

    public DataFile Current { get; private set; } = new("", new Lazy<Stream>(new MemoryStream()));

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        foreach (var dataFile in _dataFiles)
        {
            dataFile?.Dispose();
        }
    }
}