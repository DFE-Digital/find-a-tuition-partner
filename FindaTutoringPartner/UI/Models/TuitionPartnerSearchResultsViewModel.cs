using Domain;
using Domain.Search;

namespace UI.Models;

public class TuitionPartnerSearchResultsViewModel
{
    public Guid SearchId { get; set; }
    public LocationFilterParameters? LocationFilterParameters { get; set; }
    public ICollection<int> SubjectIds { get; set; } = null!;
    public IEnumerable<Subject> Subjects { get; set; } = null!;
    public ICollection<int> TutorTypeIds { get; set; } = null!;
    public IEnumerable<TutorType> TutorTypes { get; set; } = null!;
    public ICollection<int> TuitionTypeIds { get; set; } = null!;
    public IEnumerable<TuitionType> TuitionTypes { get; set; } = null!;
    public SearchResultsPage<TuitionPartnerSearchRequest, TuitionPartner> SearchResultsPage { get; set; } = null!;
}