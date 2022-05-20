namespace Domain.Search;

public class SearchState
{
    public Guid SearchId { get; set; }
    public LocationFilterParameters? LocationFilterParameters { get; set; }
    public IDictionary<int, string>? Subjects { get; set; }
    public IDictionary<int, string>? TutorTypes { get; set; }
    public IDictionary<int, string>? TuitionTypes { get; set; }
}