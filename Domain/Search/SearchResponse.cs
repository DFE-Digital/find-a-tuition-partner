namespace Domain.Search;

public class SearchResponse<TRequest, TResult>
{
    public SearchResponse(TRequest request, int count, TResult[] results)
    {
        Request = request;
        Count = count;
        Results = results;
    }

    public TRequest Request { get; set; }
    public int Count { get; set; }
    public TResult[] Results { get; set; }
}