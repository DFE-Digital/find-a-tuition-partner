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
            .Where(e => ids.Distinct().Contains(e.Id))
            .ToListAsync(cancellationToken);

        var coverageQueryable = _dbContext.TuitionPartnerCoverage.Where(e => ids.Distinct().Contains(e.TuitionPartnerId));

        if (localAuthorityDistrictId.HasValue)
        {
            coverageQueryable = coverageQueryable.Where(e => e.LocalAuthorityDistrictId == localAuthorityDistrictId.Value);
        }

        var coverageDictionary = (await coverageQueryable.Select(e => new
            {
                e.TuitionPartnerId,
                e.TuitionTypeId,
                e.PrimaryLiteracy,
                e.PrimaryNumeracy,
                e.PrimaryScience,
                e.SecondaryEnglish,
                e.SecondaryHumanities,
                e.SecondaryMaths,
                e.SecondaryModernForeignLanguages,
                e.SecondaryScience
            }).Distinct().ToListAsync(cancellationToken))
            .GroupBy(e => e.TuitionPartnerId).ToDictionary(e => e.Key, e => e.ToArray());

        var subjectDictionary = (await _lookupDataRepository.GetSubjectsAsync(cancellationToken)).ToDictionary(e => e.Id);
        var tuitionTypeDictionary = (await _lookupDataRepository.GetTuitionTypesAsync(cancellationToken)).ToDictionary(e => e.Id);

        var results = new List<TuitionPartnerSearchResult>(entities.Count);
        foreach (var entity in entities)
        {
            var result = entity.Adapt<TuitionPartnerSearchResult>();
            result.Description = string.IsNullOrWhiteSpace(result.Description) ? $"{result.Name} description placeholder" : result.Description;

            var coverage = coverageDictionary[result.Id];

            var subjects = new List<Subject>();
            if (coverage.Any(e => e.PrimaryLiteracy)) subjects.Add(subjectDictionary[Subjects.Id.PrimaryLiteracy]);
            if (coverage.Any(e => e.PrimaryNumeracy)) subjects.Add(subjectDictionary[Subjects.Id.PrimaryNumeracy]);
            if (coverage.Any(e => e.PrimaryScience)) subjects.Add(subjectDictionary[Subjects.Id.PrimaryScience]);
            if (coverage.Any(e => e.SecondaryEnglish)) subjects.Add(subjectDictionary[Subjects.Id.SecondaryEnglish]);
            if (coverage.Any(e => e.SecondaryHumanities)) subjects.Add(subjectDictionary[Subjects.Id.SecondaryHumanities]);
            if (coverage.Any(e => e.SecondaryMaths)) subjects.Add(subjectDictionary[Subjects.Id.SecondaryMaths]);
            if (coverage.Any(e => e.SecondaryModernForeignLanguages)) subjects.Add(subjectDictionary[Subjects.Id.SecondaryModernForeignLanguages]);
            if (coverage.Any(e => e.SecondaryScience)) subjects.Add(subjectDictionary[Subjects.Id.SecondaryScience]);
            result.Subjects = subjects.ToArray();
            
            var resultTuitionTypes = new List<TuitionType>();
            if (coverage.Any(e => e.TuitionTypeId == TuitionTypes.Id.Online && (e.PrimaryLiteracy || e.PrimaryNumeracy || e.PrimaryScience || e.SecondaryEnglish || e.SecondaryHumanities || e.SecondaryMaths || e.SecondaryModernForeignLanguages || e.SecondaryScience))) resultTuitionTypes.Add(tuitionTypeDictionary[TuitionTypes.Id.Online]);
            if (coverage.Any(e => e.TuitionTypeId == TuitionTypes.Id.InPerson && (e.PrimaryLiteracy || e.PrimaryNumeracy || e.PrimaryScience || e.SecondaryEnglish || e.SecondaryHumanities || e.SecondaryMaths || e.SecondaryModernForeignLanguages || e.SecondaryScience))) resultTuitionTypes.Add(tuitionTypeDictionary[TuitionTypes.Id.InPerson]);
            result.TuitionTypes = resultTuitionTypes.ToArray();

            results.Add(result);
        }

        switch (orderBy)
        {
            case TuitionPartnerOrderBy.Name:
                return direction == OrderByDirection.Descending
                    ? results.OrderByDescending(e => e.Name).ToDictionary(e => e.Id)
                    : results.OrderBy(e => e.Name).ToDictionary(e => e.Id);
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