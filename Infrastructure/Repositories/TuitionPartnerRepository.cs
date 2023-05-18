using Application.Common.Interfaces.Repositories;
using Domain;
using Domain.Search;
using Infrastructure.Mapping;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Repositories;

public class TuitionPartnerRepository : GenericRepository<TuitionPartner>, ITuitionPartnerRepository
{
    public TuitionPartnerRepository(NtpDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<int[]?> GetTuitionPartnersFilteredAsync(TuitionPartnersFilter filter, CancellationToken cancellationToken)
    {
        var queryable = _context.TuitionPartners.Where(x => x.IsActive).AsQueryable();

        if (filter.SeoUrls is not null)
        {
            queryable = queryable.Where(e => filter.SeoUrls.Contains(e.SeoUrl));
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            queryable = queryable.Where(e => e.Name.ToLower().Contains(filter.Name.ToLower()));
        }

        if (filter.LocalAuthorityDistrictId != null)
        {
            queryable = queryable.Where(e => e.LocalAuthorityDistrictCoverage.Any(x => x.LocalAuthorityDistrictId == filter.LocalAuthorityDistrictId && (filter.TuitionSettingId == null || x.TuitionSettingId == filter.TuitionSettingId)));
        }
        else if (filter.TuitionSettingId != null)
        {
            queryable = queryable.Where(e => e.LocalAuthorityDistrictCoverage.Any(x => x.TuitionSettingId == filter.TuitionSettingId));
        }

        if (filter.SubjectIds != null)
        {
            foreach (var subjectId in filter.SubjectIds)
            {
                // Must support all selected subjects for the tuition setting if selected
                // TODO: This is a slow query that gets worse as multiple subjects are selected. Will need optimising, possibly by denormalising the data
                queryable = queryable.Where(e => e.SubjectCoverage.Any(x => x.SubjectId == subjectId && (filter.TuitionSettingId == null || x.TuitionSettingId == filter.TuitionSettingId)));
            }
        }
        else if (filter.TuitionSettingId != null)
        {
            queryable = queryable.Where(e => e.SubjectCoverage.Any(x => x.TuitionSettingId == filter.TuitionSettingId));
        }

        var ids = await queryable.Select(e => e.Id).ToArrayAsync(cancellationToken);

        return ids;
    }

    public async Task<IEnumerable<TuitionPartnerResult>> GetTuitionPartnersAsync(TuitionPartnerRequest request, CancellationToken cancellationToken)
    {
        var results = new List<TuitionPartnerResult>();

        if (request.TuitionPartnerIds == null || request.TuitionPartnerIds.Length > 0)
        {
            TuitionPartnerMapping.Configure();

            var entities = await _context.TuitionPartners.AsNoTracking()
                .Include(x => x.OrganisationType)
                .IncludeTuitionForLocalDistrict(request.LocalAuthorityDistrictId)
                .ThenInclude(x => x.TuitionSetting)
                .Include(x => x.SubjectCoverage)
                .ThenInclude(x => x.Subject)
                .ThenInclude(x => x.KeyStage)
                .Include(x => x.SubjectCoverage)
                .ThenInclude(x => x.TuitionSetting)
                .Include(x => x.Prices)
                .ThenInclude(x => x.TuitionSetting)
                .Include(x => x.Prices)
                .ThenInclude(x => x.Subject)
                .ThenInclude(x => x.KeyStage)
                .Where(x => (request.TuitionPartnerIds == null && x.IsActive) ||
                            (request.TuitionPartnerIds != null && request.TuitionPartnerIds.Distinct().Contains(x.Id)))
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            foreach (var entity in entities)
            {
                var result = entity.Adapt<TuitionPartnerResult>();

                result.TuitionSettings = entity.LocalAuthorityDistrictCoverage.Select(e => e.TuitionSetting).OrderByDescending(e => e.Id).Distinct().ToArray();

                var tuitionSettingIds = result.TuitionSettings.Select(e => e.Id);

                result.SubjectsCoverage = entity.SubjectCoverage.Where(e => tuitionSettingIds.Contains(e.TuitionSetting.Id)).OrderBy(e => e.Id).Distinct().ToArray();

                result.Prices = entity.Prices.Where(e => tuitionSettingIds.Contains(e.TuitionSetting.Id)).OrderBy(e => e.Id).Distinct().ToArray();

                results.Add(result);
            }
        }

        return results;
    }
}

public static class LocalAuthorityDistrictCoverageQueryExtension
{
    public static IIncludableQueryable<TuitionPartner, IEnumerable<LocalAuthorityDistrictCoverage>>
    IncludeTuitionForLocalDistrict(this IQueryable<TuitionPartner> tuitionPartners, int? localAuthorityDistrictId)
    {
        return localAuthorityDistrictId == null
            ? tuitionPartners.Include(e => e.LocalAuthorityDistrictCoverage)
            : tuitionPartners.Include(e => e.LocalAuthorityDistrictCoverage.Where(lad => lad.LocalAuthorityDistrictId == localAuthorityDistrictId));
    }
}