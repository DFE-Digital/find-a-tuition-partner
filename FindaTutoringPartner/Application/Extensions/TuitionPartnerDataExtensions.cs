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
                if (intoTuitionPartner.Coverage.Any(e => 
                        e.LocalAuthorityDistrict.Equals(coverage.LocalAuthorityDistrict)
                        && e.Subject.Equals(coverage.Subject)
                        && e.TuitionType.Equals(coverage.TuitionType))) continue;

                intoTuitionPartner.Coverage.Add(coverage);
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
                Id = target.Id,
                Name = existing.Name,
                Website = existing.Website
            };

            foreach (var targetCoverage in target.Coverage)
            {
                if (existing.Coverage.Any(e =>
                        e.LocalAuthorityDistrict.Equals(targetCoverage.LocalAuthorityDistrict)
                        && e.Subject.Equals(targetCoverage.Subject)
                        && e.TuitionType.Equals(targetCoverage.TuitionType))) continue;

                delta.CoverageAdd.Add(targetCoverage);
            }

            foreach (var existingCoverage in existing.Coverage)
            {
                if (target.Coverage.Any(e =>
                        e.LocalAuthorityDistrict.Equals(existingCoverage.LocalAuthorityDistrict)
                        && e.Subject.Equals(existingCoverage.Subject)
                        && e.TuitionType.Equals(existingCoverage.TuitionType))) continue;

                delta.CoverageRemove.Add(existingCoverage);
            }

            deltas.Update.Add(delta);
        }

        return deltas;
    }
}