using Application.Common.Interfaces.Repositories;
using Domain;
using Domain.Search;
using Infrastructure.Mapping;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TuitionSetting = Domain.Enums.TuitionSetting;

namespace Infrastructure.Repositories;

public class TuitionPartnerRepository : GenericRepository<TuitionPartner>, ITuitionPartnerRepository
{
    public TuitionPartnerRepository(NtpDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<int[]?> GetTuitionPartnersFilteredAsync(TuitionPartnersFilter filter, CancellationToken cancellationToken)
    {
        var queryable = _context.TuitionPartners.Where(x => x.IsActive).AsQueryable();
        var tuitionSettingIds = filter == null || filter.TuitionSettingId == null || filter.TuitionSettingId == (int)TuitionSetting.NoPreference ? new List<int>() :
            filter.TuitionSettingId == (int)TuitionSetting.Both ? new List<int>() { (int)TuitionSetting.Online, (int)TuitionSetting.FaceToFace } :
            new List<int>() { (int)filter.TuitionSettingId };

        if (filter!.SeoUrls is not null)
        {
            queryable = queryable.Where(e => filter.SeoUrls.Contains(e.SeoUrl));
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            queryable = queryable.Where(e => e.Name.ToLower().Contains(filter.Name.ToLower()));
        }

        if (filter.LocalAuthorityDistrictId != null || tuitionSettingIds.Any())
        {
            if (tuitionSettingIds.Count <= 1)
            {
                queryable = queryable
                    .Where(e => e.LocalAuthorityDistrictCoverage
                        .Any(x =>
                            (filter.LocalAuthorityDistrictId == null || x.LocalAuthorityDistrictId == filter.LocalAuthorityDistrictId) &&
                            (!tuitionSettingIds.Any() || tuitionSettingIds[0] == x.TuitionSettingId)
                        )
                    );
            }
            else
            {
                queryable = queryable
                    .Where(e => e.LocalAuthorityDistrictCoverage
                        .GroupBy(x => x.LocalAuthorityDistrictId)
                        .Any(x => (filter.LocalAuthorityDistrictId == null || x.Key == filter.LocalAuthorityDistrictId) &&
                            x.Count() == tuitionSettingIds.Count
                        )
                    );
            }
        }

        if (filter.SubjectIds != null)
        {
            foreach (var subjectId in filter.SubjectIds)
            {
                // Must support all selected subjects for the tuition setting if selected
                // TODO: This is a slow query that gets worse as multiple subjects are selected. Will need optimising, possibly by denormalising the data
                queryable = AddSubjectCoverageToQuery(queryable, subjectId, tuitionSettingIds);
            }
        }
        else if (tuitionSettingIds.Any())
        {
            queryable = AddSubjectCoverageToQuery(queryable, null, tuitionSettingIds);
        }

        var ids = await queryable.Select(e => e.Id).ToArrayAsync(cancellationToken);

        return ids;
    }

    private static IQueryable<TuitionPartner> AddSubjectCoverageToQuery(IQueryable<TuitionPartner> queryable, int? subjectId, List<int> tuitionSettingIds)
    {
        if (tuitionSettingIds.Count <= 1)
        {
            queryable = queryable
                .Where(e => e.SubjectCoverage
                    .Any(x => (subjectId == null || x.SubjectId == subjectId) &&
                        (!tuitionSettingIds.Any() || tuitionSettingIds[0] == x.TuitionSettingId)
                    )
                );
        }
        else
        {
            queryable = queryable
                .Where(e => e.SubjectCoverage.GroupBy(x => x.SubjectId)
                    .Any(x => (subjectId == null || x.Key == subjectId) &&
                        x.Count() == tuitionSettingIds.Count)
                );
        }
        return queryable;
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