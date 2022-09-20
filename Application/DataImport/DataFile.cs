namespace Application.DataImport;

public record DataFile(string Filename, Stream Stream) : IDisposable
{
    public void Dispose()
    {
        Stream.Dispose();
    }
}
