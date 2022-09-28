namespace Application.DataImport;

public sealed record DataFile(string Filename, Lazy<Stream> Stream) : IDisposable
{
    public void Dispose()
    {
        if (Stream.IsValueCreated)
            Stream.Value.Dispose();
    }
}