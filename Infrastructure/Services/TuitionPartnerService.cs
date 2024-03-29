﻿using Application.Common.Interfaces;
using Application.Extensions;
using Domain.Enums;
using Domain.Search;

namespace Infrastructure.Services;

public class TuitionPartnerService : ITuitionPartnerService
{
    private readonly IUnitOfWork _unitOfWork;

    public TuitionPartnerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int[]?> GetTuitionPartnersFilteredAsync(TuitionPartnersFilter filter, CancellationToken cancellationToken)
    {
        return await _unitOfWork.TuitionPartnerRepository.GetTuitionPartnersFilteredAsync(filter, cancellationToken);
    }

    public async Task<IEnumerable<TuitionPartnerResult>> GetTuitionPartnersAsync(TuitionPartnerRequest request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.TuitionPartnerRepository.GetTuitionPartnersAsync(request, cancellationToken);
    }

    public IEnumerable<TuitionPartnerResult> FilterTuitionPartnersData(IEnumerable<TuitionPartnerResult> results, TuitionPartnersDataFilter dataFilter)
    {
        //Remove the pricing, tuition setting and subject data for all TPs that are not in the data filter supplied
        foreach (var tpResult in results)
        {
            tpResult.RefinedDataEmptyReason = string.Empty;
            var prices = tpResult.Prices!.ToList();
            if (prices.Any())
            {
                if (dataFilter.SubjectIds != null && dataFilter.SubjectIds.Any())
                {
                    //Filter prices so only include the subjects
                    if (prices.Any())
                    {
                        prices = prices.Where(x => dataFilter.SubjectIds.Contains(x.SubjectId)).ToList();
                    }

                    //If any subject ids are not included then remove all and add empty reason
                    if (dataFilter.SubjectIds.Any(x => !prices.Select(p => p.SubjectId).Contains(x)))
                    {
                        prices = new List<Domain.Price>();
                    }

                    if (!prices.Any())
                    {
                        tpResult.RefinedDataEmptyReason = "Does not offer tuition for all the selected subjects";
                    }
                }

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

                    if (dataFilter.TuitionSettingId != null)
                    {
                        var tuitionSetting = (TuitionSetting)dataFilter.TuitionSettingId;

                        var tuitionSettingIdsForFilter = tuitionSetting == TuitionSetting.NoPreference ? new List<int>() :
                            tuitionSetting == TuitionSetting.Both ? new List<int>() { (int)TuitionSetting.Online, (int)TuitionSetting.FaceToFace } :
                            new List<int>() { dataFilter.TuitionSettingId.Value };

                        if (tuitionSettingIdsForFilter.Any(x => !pricesOriginal.Select(p => p.TuitionSettingId).Contains(x)))
                        {
                            tpResult.RefinedDataEmptyReason = string.IsNullOrEmpty(tpResult.RefinedDataEmptyReason) ?
                                $"Does not offer {tuitionSetting.DisplayName().ToLower()} tuition in " :
                                $"{tpResult.RefinedDataEmptyReason} or {tuitionSetting.DisplayName().ToLower()} tuition in ";
                            tpResult.RefinedDataEmptyReasonAppendLAName = true;
                            prices = new List<Domain.Price>();
                        }

                        //Filter prices so only include the tuition settings
                        if (prices.Any())
                        {
                            prices = prices.Where(x => tuitionSettingIdsForFilter.Contains(x.TuitionSettingId)).ToList();

                            //If any tuition setting ids are not included then remove all
                            if (prices.Any() && tuitionSettingIdsForFilter.Any(x => !prices.Select(p => p.TuitionSettingId).Contains(x)))
                            {
                                prices = new List<Domain.Price>();
                            }
                        }
                    }
                }
            }

            if (prices.Any())
            {
                tpResult.Prices = prices.ToArray();

                if (tpResult.IsVatCharged && dataFilter.ShowWithVAT.HasValue && dataFilter.ShowWithVAT.Value)
                {
                    foreach (var price in prices)
                    {
                        price.HourlyRate = price.HourlyRate.AddVAT();
                    }
                }

                var tuitionSettings = prices.Select(x => x.TuitionSettingId).Distinct();
                var subjects = prices.Select(x => x.SubjectId).Distinct();

                tpResult.TuitionSettings = tpResult.TuitionSettings!.Where(x => tuitionSettings.Contains(x.Id)).ToArray();
                tpResult.SubjectsCoverage = tpResult.SubjectsCoverage!.Where(x => subjects.Contains(x.SubjectId) && tuitionSettings.Contains(x.TuitionSettingId)).ToArray();
            }
            else
            {
                tpResult.Prices = null;
                tpResult.TuitionSettings = null;
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

            case TuitionPartnerOrderBy.Price:
                return ordering.Direction == OrderByDirection.Descending
                    ? results
                        .OrderByDescending(e => e.Prices == null ? int.MinValue : e.Prices!.Max(x => x.HourlyRate))
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
                var tuitionPartnerWithNoDataMoveToEndOfList = 1000;
                return results
                    .OrderBy(x => x.Prices == null ? (Array.IndexOf(ordering.SeoUrlOrderBy, x.SeoUrl) + tuitionPartnerWithNoDataMoveToEndOfList) : Array.IndexOf(ordering.SeoUrlOrderBy, x.SeoUrl));

            default:
                return results.OrderByDescending(e => e.Id);
        }
    }
}