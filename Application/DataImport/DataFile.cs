namespace Application.DataImport;

public sealed record DataFile(string Filename, Stream Stream) : IDisposable
{
    public void Dispose()
    {
        Stream.Dispose();
    }
}

public sealed record DataFile2(string Filename, Lazy<Stream> Stream) : IDisposable
{
    public void Dispose()
    {
        if (Stream.IsValueCreated)
            Stream.Value.Dispose();
    }
}
