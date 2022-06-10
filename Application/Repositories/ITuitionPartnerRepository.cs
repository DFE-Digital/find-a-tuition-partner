using Domain.Deltas;
using Domain.Search;

namespace Application.Repositories;

public interface ITuitionPartnerRepository
{
    Task<IDictionary<int, TuitionPartnerSearchResult>> GetSearchResultsDictionaryAsync(IEnumerable<int> ids, int? localAuthorityDistrictId, TuitionPartnerOrderBy orderBy, OrderByDirection direction, CancellationToken cancellationToken = default);
    Task ApplyDeltas(TuitionPartnerDeltas deltas);
}