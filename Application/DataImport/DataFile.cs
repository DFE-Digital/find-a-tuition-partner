namespace Application.DataImport;

public sealed record DataFile(string Filename, Lazy<Stream> Stream) : IDisposable
{
    public DataFile(string filename) : this(filename, new Lazy<Stream>()) { }
    public DataFile(string filename, Stream stream) : this(filename, new Lazy<Stream>(stream)) { }

    public string FileExtension => Path.GetExtension(Filename);

    public void Dispose()
    {
        if (Stream.IsValueCreated)
            Stream.Value.Dispose();
    }
}

