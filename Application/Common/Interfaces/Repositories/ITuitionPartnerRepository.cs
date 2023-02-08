using Domain;
using Domain.Search;

namespace Application.Common.Interfaces.Repositories;

public interface ITuitionPartnerRepository : IGenericRepository<TuitionPartner>
{
    Task<int[]?> GetTuitionPartnersFilteredAsync(TuitionPartnersFilter filter, CancellationToken cancellationToken);

    Task<IEnumerable<TuitionPartnerResult>> GetTuitionPartnersAsync(TuitionPartnerRequest request, CancellationToken cancellationToken);
}
