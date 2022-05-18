namespace Domain.Search;

public class SearchState
{
    public Guid SearchId { get; set; }
    public LocationFilterParameters? LocationFilterParameters { get; set; }
}