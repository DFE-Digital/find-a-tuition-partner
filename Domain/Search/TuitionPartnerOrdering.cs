namespace Domain.Search;

public class TuitionPartnerOrdering
{
    private readonly TuitionPartnerSearchRequest request;

    public TuitionPartnerOrdering(TuitionPartnerSearchRequest searchRequest)
        => request = searchRequest;

    public IEnumerable<TuitionPartnerSearchResult> Order(List<TuitionPartnerSearchResult> results)
    {
        switch (request.OrderBy)
        {
            case TuitionPartnerOrderBy.Name:
                return request.Direction == OrderByDirection.Descending
                    ? results.OrderByDescending(e => e.Name)
                    : results.OrderBy(e => e.Name);

            case TuitionPartnerOrderBy.Random:
                var random = new Random(RandomSeed());
                return results.OrderByDescending(e => e.Id).OrderBy(x => random.Next());

            default:
                return results.OrderByDescending(e => e.Id);
        }
    }

    public int RandomSeed()
    {
        return
            (request.LocalAuthorityDistrictCode?.Sum(x => x) ?? 0)
            + (request.SubjectIds?.Sum() ?? 0)
            + (request.TuitionTypeId ?? 0);
    }
}