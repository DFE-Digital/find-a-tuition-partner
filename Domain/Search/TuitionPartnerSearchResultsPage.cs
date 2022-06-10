namespace Domain.Search;

public class TuitionPartnerSearchResultsPage : SearchResultsPage<TuitionPartnerSearchRequest, TuitionPartnerSearchResult>
{
    public TuitionPartnerSearchResultsPage(TuitionPartnerSearchRequest request, int count, TuitionPartnerSearchResult[] results, LocalAuthorityDistrict? localAuthorityDistrict) : base(request, count, results)
    {
        LocalAuthorityDistrict = localAuthorityDistrict;
    }

    public LocalAuthorityDistrict? LocalAuthorityDistrict { get; set; }
}