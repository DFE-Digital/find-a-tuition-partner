using Application;
using Application.Repositories;
using Domain.Enums;
using Domain.Search;

namespace Infrastructure;

public class TuitionPartnerService : ITuitionPartnerService
{
    private readonly ITuitionPartnerRepository _tuitionPartnerRepositoryRepository;

    public TuitionPartnerService(ITuitionPartnerRepository tuitionPartnerRepositoryRepository)
    {
        _tuitionPartnerRepositoryRepository = tuitionPartnerRepositoryRepository;
    }

    public async Task<int[]?> GetTuitionPartnersFilteredAsync(TuitionPartnersFilter filter, CancellationToken cancellationToken)
    {
        return await _tuitionPartnerRepositoryRepository.GetTuitionPartnersFilteredAsync(filter, cancellationToken);
    }

    public async Task<IEnumerable<TuitionPartnerResult>> GetTuitionPartnersAsync(TuitionPartnerRequest request, CancellationToken cancellationToken)
    {
        return await _tuitionPartnerRepositoryRepository.GetTuitionPartnersAsync(request, cancellationToken);
    }

    public IEnumerable<TuitionPartnerResult> OrderTuitionPartners(IEnumerable<TuitionPartnerResult> results, TuitionPartnerOrderBy orderBy = TuitionPartnerOrderBy.Name, OrderByDirection orderByDirection = OrderByDirection.Ascending, int? randomSeed = null)
    {
        switch (orderBy)
        {
            case TuitionPartnerOrderBy.Name:
                return orderByDirection == OrderByDirection.Descending
                    ? results.OrderByDescending(e => e.Name)
                    : results.OrderBy(e => e.Name);

            case TuitionPartnerOrderBy.Random:
                var random = randomSeed is null ? new Random() : new Random(randomSeed.Value);
                return results.OrderByDescending(e => e.SeoUrl).OrderBy(x => random.Next());

            case TuitionPartnerOrderBy.MinPrice:
                return orderByDirection == OrderByDirection.Descending
                    ? results.OrderBy(e => e.Name).OrderByDescending(e => e.Prices.Min(x => x.HourlyRate))
                    : results.OrderBy(e => e.Name).OrderBy(e => e.Prices.Min(x => x.HourlyRate));

            default:
                return results.OrderByDescending(e => e.Id);
        }
    }
}