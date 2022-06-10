namespace Application.Exceptions;

public class SearchStateNotFoundException : Exception
{
    public SearchStateNotFoundException(Guid searchId)
    {
        SearchId = searchId;
    }

    public Guid SearchId { get; }
}