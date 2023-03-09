namespace Domain.Search;

public class TuitionPartnersResult
{
    public TuitionPartnersResult(TuitionPartnerResult result, string? localAuthorityName, string? localAuthorityDistrictName)
    {
        Results = new List<TuitionPartnerResult>() { result };
        Count = 1;
        LocalAuthorityName = localAuthorityName;
        LocalAuthorityDistrictName = localAuthorityDistrictName;
    }

    public TuitionPartnersResult(IEnumerable<TuitionPartnerResult> results, string? localAuthorityName, string? localAuthorityDistrictName)
    {
        Results = results;
        Count = results.Count();
        LocalAuthorityName = localAuthorityName;
        LocalAuthorityDistrictName = localAuthorityDistrictName;
    }

    public IEnumerable<TuitionPartnerResult> Results { get; set; }
    public int Count { get; set; }
    public string? LocalAuthorityName { get; set; }

    public string? LocalAuthorityDistrictName { get; set; }

    public TuitionPartnerResult FirstResult => Results.Any() ? Results.First() : new TuitionPartnerResult();
}