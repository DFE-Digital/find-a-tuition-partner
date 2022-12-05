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

    public IEnumerable<TuitionPartnerResult> OrderTuitionPartners(IEnumerable<TuitionPartnerResult> results, TuitionPartnerOrdering ordering)
    {
        switch (ordering.OrderBy)
        {
            case TuitionPartnerOrderBy.Name:
                return ordering.Direction == OrderByDirection.Descending
                    ? results.OrderByDescending(e => e.Name)
                    : results.OrderBy(e => e.Name);

            case TuitionPartnerOrderBy.Random:
                var random = ordering.RandomSeed is null ? new Random() : new Random(ordering.RandomSeed.Value);
                return results.OrderByDescending(e => e.SeoUrl).OrderBy(x => random.Next()).ToList();

            case TuitionPartnerOrderBy.MinPrice:
                return ordering.Direction == OrderByDirection.Descending
                    ? results.OrderBy(e => e.Name).OrderByDescending(e => e.Prices.Min(x => x.HourlyRate))
                    : results.OrderBy(e => e.Name).OrderBy(e => e.Prices.Min(x => x.HourlyRate));

            case TuitionPartnerOrderBy.SeoList:
                if (ordering.SeoUrlOrderBy == null || ordering.SeoUrlOrderBy.Length == 0)
                {
                    return results;
                }
                return results.OrderBy(x => Array.IndexOf(ordering.SeoUrlOrderBy, x.SeoUrl));

            default:
                return results.OrderByDescending(e => e.Id);
        }
    }
}