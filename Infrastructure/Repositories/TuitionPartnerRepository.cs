using Application.Repositories;
using Domain.Search;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

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
            .IncludeTuitionForLocalDistrict(localAuthorityDistrictId)
            .ThenInclude(e => e.TuitionType)
            .Include(e => e.SubjectCoverage)
            .ThenInclude(e => e.Subject)
            .Where(e => ids.Distinct().Contains(e.Id))
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        var results = new List<TuitionPartnerSearchResult>(entities.Count);
        foreach (var entity in entities)
        {
            var result = entity.Adapt<TuitionPartnerSearchResult>();
            result.Subjects = entity.SubjectCoverage.OrderBy(e => e.Id).Select(e => e.Subject).Distinct().ToArray();

            result.TuitionTypes = entity.LocalAuthorityDistrictCoverage.Select(e => e.TuitionType).OrderByDescending(e => e.Id).Distinct().ToArray();

            results.Add(result);
        }
        return ordering.Order(results);
    }
}

public static class LocalAuthorityDistrictCoverageQueryExtension
{
    public static IIncludableQueryable<Domain.TuitionPartner, IEnumerable<Domain.LocalAuthorityDistrictCoverage>>
    IncludeTuitionForLocalDistrict(this IQueryable<Domain.TuitionPartner> tuitionPartners, int? localAuthorityDistrictId)
    {
        return localAuthorityDistrictId == null
            ? tuitionPartners.Include(e => e.LocalAuthorityDistrictCoverage)
            : tuitionPartners.Include(e => e.LocalAuthorityDistrictCoverage.Where(lad => lad.LocalAuthorityDistrictId == localAuthorityDistrictId));
    }
}