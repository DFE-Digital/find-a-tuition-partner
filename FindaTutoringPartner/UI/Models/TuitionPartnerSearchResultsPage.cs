using Domain;
using Domain.Search;

namespace UI.Models;

public class TuitionPartnerSearchResultsPage : SearchResultsPage<TuitionPartnerSearchRequest, TuitionPartner>
{
    public TuitionPartnerSearchResultsPage(TuitionPartnerSearchRequest request, int count, TuitionPartner[] results) : base(request, count, results)
    {
    }
}