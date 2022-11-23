namespace Domain.Search;

public class TuitionPartnersResult
{
    public TuitionPartnersResult(IEnumerable<TuitionPartnerResult> results, int count, string? localAuthorityName)
    {
        Results = results;
        Count = count;
        LocalAuthorityName = localAuthorityName;
    }

    public IEnumerable<TuitionPartnerResult> Results { get; set; }
    public int Count { get; set; }
    public string? LocalAuthorityName { get; set; }
}