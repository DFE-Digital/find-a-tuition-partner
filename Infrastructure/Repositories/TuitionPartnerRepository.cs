using Application.Repositories;
using Domain;
using Domain.Constants;
using Domain.Deltas;
using Domain.Search;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TuitionPartnerRepository : ITuitionPartnerRepository
{
    private readonly NtpDbContext _dbContext;
    private readonly ILookupDataRepository _lookupDataRepository;

    public TuitionPartnerRepository(NtpDbContext dbContext, ILookupDataRepository lookupDataRepository)
    {
        _dbContext = dbContext;
        _lookupDataRepository = lookupDataRepository;
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

    public async Task ApplyDeltas(TuitionPartnerDeltas deltas)
    {
        foreach (var toAdd in deltas.Add)
        {
            _dbContext.TuitionPartners.Add(toAdd);
        }

        foreach (var toUpdateDelta in deltas.Update)
        {
            var toUpdate = await _dbContext.TuitionPartners.FindAsync(toUpdateDelta.Id);

            if (toUpdate == null) continue;

            toUpdate.Name = toUpdateDelta.Name;
            toUpdate.Website = toUpdateDelta.Website;

            foreach (var coverageToAdd in toUpdateDelta.CoverageAdd)
            {
                coverageToAdd.TuitionPartner = toUpdate;
                toUpdate.Coverage.Add(coverageToAdd);
            }

            foreach (var coverageToUpdateDelta in toUpdateDelta.CoverageUpdate)
            {
                var coverageToUpdate = await _dbContext.TuitionPartnerCoverage.FindAsync(coverageToUpdateDelta.Id);
                if (coverageToUpdate == null) continue;

                coverageToUpdate.PrimaryLiteracy = coverageToUpdateDelta.PrimaryLiteracy;
                coverageToUpdate.PrimaryNumeracy = coverageToUpdateDelta.PrimaryNumeracy;
                coverageToUpdate.PrimaryScience = coverageToUpdateDelta.PrimaryScience;
                coverageToUpdate.SecondaryEnglish = coverageToUpdateDelta.SecondaryEnglish;
                coverageToUpdate.SecondaryHumanities = coverageToUpdateDelta.SecondaryHumanities;
                coverageToUpdate.SecondaryMaths = coverageToUpdateDelta.SecondaryMaths;
                coverageToUpdate.SecondaryModernForeignLanguages = coverageToUpdateDelta.SecondaryModernForeignLanguages;
                coverageToUpdate.SecondaryScience = coverageToUpdateDelta.SecondaryScience;
            }

            foreach (var coverageToRemoveDelta in toUpdateDelta.CoverageRemove)
            {
                var coverageToRemove = await _dbContext.TuitionPartnerCoverage.FindAsync(coverageToRemoveDelta.Id);
                if (coverageToRemove == null) continue;

                toUpdate.Coverage.Remove(coverageToRemove);
            }
        }

        foreach (var toRemoveDelta in deltas.Remove)
        {
            var toRemove = await _dbContext.TuitionPartners.FindAsync(toRemoveDelta.Id);
            if (toRemove == null) continue;

            _dbContext.TuitionPartners.Remove(toRemove);
        }

        await _dbContext.SaveChangesAsync();
    }
}