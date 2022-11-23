using Domain.Enums;

namespace Domain.Search;

public class TuitionPartnerOrdering
{
    private readonly TuitionPartnerSearchRequest _request;

    public TuitionPartnerOrdering(TuitionPartnerSearchRequest searchRequest)
        => _request = searchRequest;

    public IEnumerable<TuitionPartnerSearchResult> Order(List<TuitionPartnerSearchResult> results)
    {
        switch (_request.OrderBy)
        {
            case TuitionPartnerOrderBy.Name:
                return _request.Direction == OrderByDirection.Descending
                    ? results.OrderByDescending(e => e.Name)
                    : results.OrderBy(e => e.Name);

            case TuitionPartnerOrderBy.Random:
                var random = new Random(RandomSeed());
                return results.OrderByDescending(e => e.SeoUrl).OrderBy(x => random.Next());

            default:
                return results.OrderByDescending(e => e.Id);
        }
    }

    public int RandomSeed()
    {
        return
            (_request.LocalAuthorityDistrictCode?.Sum(x => x) ?? 0)
            + (_request.Postcode?.Sum(x => x) ?? 0)
            + (_request.SubjectIds?.Sum() ?? 0)
            + (_request.TuitionTypeId ?? 0);
    }
}