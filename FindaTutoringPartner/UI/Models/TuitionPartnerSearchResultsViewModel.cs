using Domain;
using Domain.Search;

namespace UI.Models;

public class TuitionPartnerSearchResultsViewModel
{
    public SearchResultsPage<TuitionPartnerSearchRequest, TuitionPartner> SearchResultsPage { get; set; } = null!;
    public IEnumerable<Subject> Subjects { get; set; } = null!;
}