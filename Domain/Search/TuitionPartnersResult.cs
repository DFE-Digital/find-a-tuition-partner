namespace Domain.Search;

public class TuitionPartnersResult
{
    public TuitionPartnersResult(TuitionPartnerResult result, string? localAuthorityName)
    {
        Results = new List<TuitionPartnerResult>() { result };
        Count = 1;
        LocalAuthorityName = localAuthorityName;
    }

    public TuitionPartnersResult(IEnumerable<TuitionPartnerResult> results, string? localAuthorityName)
    {
        Results = results;
        Count = results.Count();
        LocalAuthorityName = localAuthorityName;
    }

    public IEnumerable<TuitionPartnerResult> Results { get; set; }
    public int Count { get; set; }
    public string? LocalAuthorityName { get; set; }

    public TuitionPartnerResult FirstResult
    {
        get
        {
            return Results.Any() ? Results.First() : new TuitionPartnerResult();
        }
    }
}