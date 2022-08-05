using Domain.Search;

namespace Application.Repositories;

public interface ITuitionPartnerRepository
{
    Task<IEnumerable<TuitionPartnerSearchResult>> GetSearchResultsDictionaryAsync(
        IEnumerable<int> ids, int? localAuthorityDistrictId,
        TuitionPartnerOrdering ordering, CancellationToken cancellationToken = default);
}
