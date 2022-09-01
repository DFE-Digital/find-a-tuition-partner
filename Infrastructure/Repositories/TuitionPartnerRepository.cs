using Application.Repositories;
using Domain.Search;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TuitionPartnerRepository : ITuitionPartnerRepository
{
    private readonly NtpDbContext _dbContext;

    public TuitionPartnerRepository(NtpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TuitionPartnerSearchResult>> GetSearchResultsDictionaryAsync(
        IEnumerable<int> ids,
        int? localAuthorityDistrictId,
        TuitionPartnerOrdering ordering,
        CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.TuitionPartners.AsNoTracking()
            .Include(e => e.LocalAuthorityDistrictCoverage.Where(lad => lad.LocalAuthorityDistrictId == localAuthorityDistrictId))
            .ThenInclude(e => e.TuitionType)
            .Include(e => e.SubjectCoverage)
            .ThenInclude(e => e.Subject)
            .Where(e => ids.Distinct().Contains(e.Id))
            .ToListAsync(cancellationToken);

        var results = new List<TuitionPartnerSearchResult>(entities.Count);
        foreach (var entity in entities)
        {
            var result = entity.Adapt<TuitionPartnerSearchResult>();
            result.Subjects = entity.SubjectCoverage.OrderBy(e => e.Id).Select(e => e.Subject).Distinct().ToArray();

            if (entity.LocalAuthorityDistrictCoverage.Count != 0)
            {
                result.TuitionTypes = entity.LocalAuthorityDistrictCoverage.Select(e => e.TuitionType).OrderByDescending(e => e.Id).Distinct().ToArray();
            }
            else 
            {
                result.TuitionTypes = _dbContext.LocalAuthorityDistrictCoverage.Select(e => e.TuitionType).OrderByDescending(e => e.Id).Distinct().ToArray();
            }

            results.Add(result);
        }
        return ordering.Order(results);
    }
}