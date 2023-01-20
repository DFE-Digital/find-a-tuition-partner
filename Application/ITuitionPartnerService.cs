using Domain.Enums;
using Domain.Search;

namespace Application;

public interface ITuitionPartnerService
{
    Task<int[]?> GetTuitionPartnersFilteredAsync(TuitionPartnersFilter filter, CancellationToken cancellationToken);
    Task<IEnumerable<TuitionPartnerResult>> GetTuitionPartnersAsync(TuitionPartnerRequest request, CancellationToken cancellationToken);
    IEnumerable<TuitionPartnerResult> FilterTuitionPartnersData(IEnumerable<TuitionPartnerResult> results, TuitionPartnersDataFilter dataFilter);
    IEnumerable<TuitionPartnerResult> OrderTuitionPartners(IEnumerable<TuitionPartnerResult> results, TuitionPartnerOrdering ordering);
}