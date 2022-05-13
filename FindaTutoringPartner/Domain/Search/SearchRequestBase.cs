namespace Domain.Search;

public abstract class SearchRequestBase
{
    public const int DefaultPageSize = 50;
    
    public int Page { get; set; }
    public int PageSize { get; set; } = DefaultPageSize;
}