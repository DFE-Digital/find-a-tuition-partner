using Domain;
using Domain.Deltas;

namespace Application.Extensions;

public static class TuitionPartnerDataExtensions
{
    public static IEnumerable<TuitionPartner> Combine(this IEnumerable<TuitionPartner> into, IEnumerable<TuitionPartner> from)
    {
        var combined = into.ToList();

        foreach (var fromTuitionPartner in from)
        {
            var intoTuitionPartner = combined.SingleOrDefault(e => e.Name == fromTuitionPartner.Name);

            if (intoTuitionPartner == null)
            {
                combined.Add(fromTuitionPartner);
                continue;
            }

            foreach (var coverage in fromTuitionPartner.Coverage)
            {
                var intoCoverage = intoTuitionPartner.Coverage.SingleOrDefault(e =>
                    e.LocalAuthorityDistrictId == coverage.LocalAuthorityDistrictId &&
                    e.TuitionTypeId == coverage.TuitionTypeId);

                if (intoCoverage == null)
                {
                    intoTuitionPartner.Coverage.Add(coverage);
                }
                else
                {
                    intoCoverage.PrimaryLiteracy = intoCoverage.PrimaryLiteracy || coverage.PrimaryLiteracy;
                    intoCoverage.PrimaryNumeracy = intoCoverage.PrimaryNumeracy || coverage.PrimaryNumeracy;
                    intoCoverage.PrimaryScience = intoCoverage.PrimaryScience || coverage.PrimaryScience;
                    intoCoverage.SecondaryEnglish = intoCoverage.SecondaryEnglish || coverage.SecondaryEnglish;
                    intoCoverage.SecondaryHumanities = intoCoverage.SecondaryHumanities || coverage.SecondaryHumanities;
                    intoCoverage.SecondaryMaths = intoCoverage.SecondaryMaths || coverage.SecondaryMaths;
                    intoCoverage.SecondaryModernForeignLanguages = intoCoverage.SecondaryModernForeignLanguages || coverage.SecondaryModernForeignLanguages;
                    intoCoverage.SecondaryScience = intoCoverage.SecondaryScience || coverage.SecondaryScience;
                }
            }
        }

        return combined;
    }

    public static TuitionPartnerDeltas GetDeltas(this IEnumerable<TuitionPartner> from, IEnumerable<TuitionPartner> to)
    {
        var deltas = new TuitionPartnerDeltas();

        var fromList = from.ToList();
        var toList = to.ToList();

        foreach (var target in toList)
        {
            if (fromList.Any(e => e.Id == target.Id || e.Name == target.Name)) continue;

            deltas.Add.Add(target);
        }

        foreach (var existing in fromList)
        {
            var target = toList.SingleOrDefault(e => e.Id == existing.Id || e.Name == existing.Name);

            if (target == null)
            {
                deltas.Remove.Add(existing);
                continue;
            }

            var delta = new TuitionPartnerDelta
            {
                Id = existing.Id,
                Name = existing.Name,
                Website = existing.Website
            };

            var update = false;

            foreach (var targetCoverage in target.Coverage)
            {
                if (existing.Coverage.Any(e => e.LocalAuthorityDistrictId == targetCoverage.LocalAuthorityDistrictId)) continue;

                targetCoverage.TuitionPartner = existing;
                delta.CoverageAdd.Add(targetCoverage);
                update = true;
            }

            foreach (var existingCoverage in existing.Coverage)
            {
                var targetCoverage = target.Coverage.SingleOrDefault(e =>
                    e.LocalAuthorityDistrictId == existingCoverage.LocalAuthorityDistrictId &&
                    e.TuitionTypeId == existingCoverage.TuitionTypeId);

                if (targetCoverage == null)
                {
                    delta.CoverageRemove.Add(existingCoverage);
                    update = true;
                    continue;
                }

                if (existingCoverage.PrimaryLiteracy != targetCoverage.PrimaryLiteracy
                    || existingCoverage.PrimaryNumeracy != targetCoverage.PrimaryNumeracy
                    || existingCoverage.PrimaryScience != targetCoverage.PrimaryScience
                    || existingCoverage.SecondaryEnglish != targetCoverage.SecondaryEnglish
                    || existingCoverage.SecondaryHumanities != targetCoverage.SecondaryHumanities
                    || existingCoverage.SecondaryMaths != targetCoverage.SecondaryMaths
                    || existingCoverage.SecondaryModernForeignLanguages != targetCoverage.SecondaryModernForeignLanguages
                    || existingCoverage.SecondaryScience != targetCoverage.SecondaryScience)
                {
                    targetCoverage.Id = existingCoverage.Id;
                    delta.CoverageUpdate.Add(targetCoverage);
                    update = true;
                }
            }

            if (update)
            {
                deltas.Update.Add(delta);
            }
        }

        return deltas;
    }
}