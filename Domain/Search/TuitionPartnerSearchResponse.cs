namespace Domain.Search;

public class TuitionPartnerSearchResponse : SearchResponse<TuitionPartnerSearchRequest, TuitionPartnerSearchResult>
{
    public TuitionPartnerSearchResponse(TuitionPartnerSearchRequest request, int count, TuitionPartnerSearchResult[] results, LocalAuthorityDistrict? localAuthorityDistrict) : base(request, count, results)
    {
        LocalAuthorityDistrict = localAuthorityDistrict;
    }

    public LocalAuthorityDistrict? LocalAuthorityDistrict { get; set; }
}