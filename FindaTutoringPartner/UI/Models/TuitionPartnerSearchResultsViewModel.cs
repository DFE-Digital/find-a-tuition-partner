using Domain;
using Domain.Search;

namespace UI.Models;

public class TuitionPartnerSearchResultsViewModel : SearchViewModelBase
{
    public TuitionPartnerSearchResultsViewModel()
    {
        LocationSearchViewModel = new LocationSearchViewModel();
        SubjectsSearchViewModel = new SubjectsSearchViewModel();
        TuitionTypeSearchViewModel = new TuitionTypeSearchViewModel();
    }

    public LocationSearchViewModel LocationSearchViewModel { get; set; }
    public SubjectsSearchViewModel SubjectsSearchViewModel { get; set; }
    public TuitionTypeSearchViewModel TuitionTypeSearchViewModel { get; set; }
    public SearchResultsPage<TuitionPartnerSearchRequest, TuitionPartner>? SearchResultsPage { get; set; }
}