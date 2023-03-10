namespace Domain.Search;

public class TuitionPartnersResult
{
    public TuitionPartnersResult(TuitionPartnerResult result, string? localAuthorityDistrictName)
    {
        Results = new List<TuitionPartnerResult>() { result };
        Count = 1;
        LocalAuthorityDistrictName = localAuthorityDistrictName;
    }

    public TuitionPartnersResult(IEnumerable<TuitionPartnerResult> results, string? localAuthorityDistrictName)
    {
        Results = results;
        Count = results.Count();
        LocalAuthorityDistrictName = localAuthorityDistrictName;
    }

    public IEnumerable<TuitionPartnerResult> Results { get; set; }
    public int Count { get; set; }
    public string? LocalAuthorityDistrictName { get; set; }

    public TuitionPartnerResult FirstResult => Results.Any() ? Results.First() : new TuitionPartnerResult();
}