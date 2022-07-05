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

    public async Task<IDictionary<int, TuitionPartnerSearchResult>> GetSearchResultsDictionaryAsync(IEnumerable<int> ids, int? localAuthorityDistrictId, TuitionPartnerOrderBy orderBy, OrderByDirection direction, CancellationToken cancellationToken = default)
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

            result.Subjects = entity.SubjectCoverage.Select(e => e.Subject).Distinct().ToArray();
            
            result.TuitionTypes = entity.LocalAuthorityDistrictCoverage.Select(e => e.TuitionType).Distinct().ToArray();

            results.Add(result);
        }
       
        switch (orderBy)
        {
            case TuitionPartnerOrderBy.Name:
                return direction == OrderByDirection.Descending
                    ? results.OrderByDescending(e => e.Name).ToDictionary(e => e.Id)
                    : results.OrderBy(e => e.Name).ToDictionary(e => e.Id);
            case TuitionPartnerOrderBy.Random:
                return results.OrderBy(_ => Guid.NewGuid()).ToDictionary(e => e.Id);
            default:
                return results.OrderByDescending(e => e.Id).ToDictionary(e => e.Id);
        }
    }
}