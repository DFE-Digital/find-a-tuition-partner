namespace Domain.Search;

public class SearchResultsPage<TRequest, TResult> where TRequest : SearchRequestBase
{
    public SearchResultsPage(TRequest request, int count, ICollection<TResult> results)
    {
        Request = request;
        Count = count;
        Results = results;
    }

    public TRequest Request { get; set; }
    public int Count { get; set; }
    public ICollection<TResult> Results { get; set; }
}