using Application;
using Application.Common.Interfaces;
using Application.Extensions;
using Application.Repositories;
using Domain.Enums;
using Domain.Search;

namespace Infrastructure.Services;

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

    public IEnumerable<TuitionPartnerResult> FilterTuitionPartnersData(IEnumerable<TuitionPartnerResult> results, TuitionPartnersDataFilter dataFilter)
    {
        //Remove the pricing, tuition type and subject data for all TPs that are not in the data filter supplied
        foreach (var tpResult in results)
        {
            tpResult.RefinedDataEmptyReason = string.Empty;
            var prices = tpResult.Prices!.ToList();
            if (prices.Any())
            {
                var pricesOriginal = tpResult.Prices!.ToList();

                if (dataFilter.GroupSize != null && tpResult.Prices != null)
                {
                    prices = prices.Where(x => x.GroupSize == dataFilter.GroupSize.Value).ToList();
                    if (!prices.Any())
                    {
                        tpResult.RefinedDataEmptyReason = $"Does not offer group sizes of {((GroupSize)dataFilter.GroupSize).DisplayName()}";
                    }
                }

                if (dataFilter.SubjectIds != null && dataFilter.SubjectIds.Any() && prices.Any())
                {
                    var refinedOriginalPrices = pricesOriginal.Where(x => dataFilter.SubjectIds.Contains(x.SubjectId)).ToList();
                    if (!refinedOriginalPrices.Any())
                    {
                        tpResult.RefinedDataEmptyReason = string.IsNullOrEmpty(tpResult.RefinedDataEmptyReason) ?
                            $"Does not offer tuition for the selected subject" :
                            $"{tpResult.RefinedDataEmptyReason} or for the selected subject";
                    }

                    if (prices.Any())
                    {
                        prices = prices.Where(x => dataFilter.SubjectIds.Contains(x.SubjectId)).ToList();
                    }
                }

                if (dataFilter.TuitionTypeId != null)
                {
                    var refinedOriginalPrices = pricesOriginal.Where(x => x.TuitionTypeId == dataFilter.TuitionTypeId.Value).ToList();
                    if (!refinedOriginalPrices.Any())
                    {
                        tpResult.RefinedDataEmptyReason = string.IsNullOrEmpty(tpResult.RefinedDataEmptyReason) ?
                            $"Does not offer {((TuitionType)dataFilter.TuitionTypeId).DisplayName().ToLower()} tuition in " :
                            $"{tpResult.RefinedDataEmptyReason} or {((TuitionType)dataFilter.TuitionTypeId).DisplayName().ToLower()} tuition in ";
                        tpResult.RefinedDataEmptyReasonAppendLAName = true;
                    }

                    if (prices.Any())
                    {
                        prices = prices.Where(x => x.TuitionTypeId == dataFilter.TuitionTypeId.Value).ToList();
                    }
                }
            }

            if (prices.Any())
            {
                tpResult.Prices = prices.ToArray();
                var tuitionTypes = prices.Select(x => x.TuitionTypeId).Distinct();
                var subjects = prices.Select(x => x.SubjectId).Distinct();

                tpResult.TuitionTypes = tpResult.TuitionTypes!.Where(x => tuitionTypes.Contains(x.Id)).ToArray();
                tpResult.SubjectsCoverage = tpResult.SubjectsCoverage!.Where(x => subjects.Contains(x.SubjectId)).ToArray();
            }
            else
            {
                tpResult.Prices = null;
                tpResult.TuitionTypes = null;
                tpResult.SubjectsCoverage = null;
                tpResult.RefinedDataEmptyReason ??= "Does not offer tuition for the selected options";
            }
        }

        return results;
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
                return results
                    .OrderBy(x => random.Next())
                    .ThenByDescending(e => e.SeoUrl)
                    .ToList();

            case TuitionPartnerOrderBy.MinPrice:
                return ordering.Direction == OrderByDirection.Descending
                    ? results
                        .OrderByDescending(e => e.Prices == null ? int.MinValue : e.Prices!.Min(x => x.HourlyRate))
                        .ThenBy(s => (ordering.SeoUrlOrderBy == null || ordering.SeoUrlOrderBy.Length == 0) ? -1 : Array.IndexOf(ordering.SeoUrlOrderBy, s.SeoUrl))
                        .ThenBy(e => e.Name)
                    : results
                        .OrderBy(e => e.Prices == null ? int.MaxValue : e.Prices!.Min(x => x.HourlyRate))
                        .ThenBy(s => (ordering.SeoUrlOrderBy == null || ordering.SeoUrlOrderBy.Length == 0) ? -1 : Array.IndexOf(ordering.SeoUrlOrderBy, s.SeoUrl))
                        .ThenBy(e => e.Name);

            case TuitionPartnerOrderBy.SeoList:
                if (ordering.SeoUrlOrderBy == null || ordering.SeoUrlOrderBy.Length == 0)
                {
                    return results;
                }
                var tuitionPartnerWithDataMoveToEndOfList = 1000;
                return results
                    .OrderBy(x => x.Prices == null ? (Array.IndexOf(ordering.SeoUrlOrderBy, x.SeoUrl) + tuitionPartnerWithDataMoveToEndOfList) : Array.IndexOf(ordering.SeoUrlOrderBy, x.SeoUrl));

            default:
                return results.OrderByDescending(e => e.Id);
        }
    }
}