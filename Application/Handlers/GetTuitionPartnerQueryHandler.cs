using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Structs;
using Application.Constants;
using Application.Extensions;
using Application.Queries;
using Domain;
using Domain.Enums;
using Domain.Search;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Enums = Domain.Enums;
using GroupSize = Domain.Enums.GroupSize;
using KeyStage = Domain.Enums.KeyStage;
using LocalAuthorityDistrictCoverage = Application.Common.Structs.LocalAuthorityDistrictCoverage;
using TuitionSetting = Domain.Enums.TuitionSetting;

namespace Application.Handlers;

public class GetTuitionPartnerQueryHandler : IRequestHandler<GetTuitionPartnerQuery, TuitionPartnerModel?>
{
    private readonly ILocationFilterService _locationService;
    private readonly ITuitionPartnerService _tuitionPartnerService;
    private readonly ILookupDataService _lookupDataService;
    private readonly INtpDbContext _db;
    private readonly ILogger<TuitionPartner> _logger;

    public GetTuitionPartnerQueryHandler(ILocationFilterService locationService, ITuitionPartnerService tuitionPartnerService,
        ILookupDataService lookupDataService, INtpDbContext db, ILogger<TuitionPartner> logger)
    {
        _locationService = locationService;
        _tuitionPartnerService = tuitionPartnerService;
        _lookupDataService = lookupDataService;
        _db = db;
        _logger = logger;
    }

    public async Task<TuitionPartnerModel?> Handle(GetTuitionPartnerQuery request, CancellationToken cancellationToken)
    {
        var tpResult = GetTpResult(request, cancellationToken);

        if (!tpResult.Result.IsSuccess) return null;

        var tp = tpResult.Result.Data.FirstResult;

        var subjects = tp.SubjectsCoverage!.Select(x => x.Subject).Distinct().GroupBy(x => x.KeyStageId)
            .Select(x => $"{((KeyStage)x.Key).DisplayName()}: {x.DisplayList()}");
        var types = tp.TuitionSettings!.Select(x => x.Name).Distinct();
        var ratios = tp.Prices!.Select(x => x.GroupSize).Distinct().Select(x => $"{((GroupSize)x).DisplayName()}");
        var prices = GetPricing(tp.Prices!);
        var lads = await GetLocalAuthorityDistricts(request, tp.Id);
        var allPrices = await GetFullPricing(request, tp.Prices!);

        return new(
            request.Id,
            tp.Name,
            tp.HasLogo,
            tp.Description,
            subjects.ToArray(),
            types!.ToArray(),
            ratios.ToArray(),
            prices,
            tp.Website,
            tp.PhoneNumber,
            tp.Email,
            tp.Address.SplitByLineBreaks(),
            tp.IsVatCharged,
            lads,
            allPrices,
            tp.OrganisationTypeName,
            tpResult.Result.Data.LocalAuthorityDistrictName,
            request.ShowFullInfo ? tp.TPLastUpdatedData : null,
            request.ShowFullInfo ? tp.ImportProcessLastUpdatedData : null,
            request.ShowFullInfo ? tp.ImportId : null,
            request.ShowFullInfo ? tp.IsActive : null);
    }

    private async Task<IResult<TuitionPartnersResult>> GetTpResult(GetTuitionPartnerQuery request,
        CancellationToken cancellationToken)
    {
        var locationResult = await GetSearchLocation(request, cancellationToken);

        LocationFilterParameters location = new();

        if (!locationResult.IsSuccess)
        {
            //Shouldn't be invalid, unless query string edited - since postcode on this page comes from previous page with validation
            _logger.LogWarning("Invalid postcode '{Postcode}' provided on TP details page", request.SearchModel?.Postcode);

            //Set to null and continue to get nationwide data
            if (request.SearchModel != null) request.SearchModel.Postcode = null;
        }
        else
        {
            location = locationResult.Data;
        }

        var tpResult = await FindTuitionPartner(
            location,
            request,
            cancellationToken);

        if (tpResult is IErrorResult tpError)
            return tpError.Cast<TuitionPartnersResult>();

        var result = new TuitionPartnersResult(tpResult.Data, location.LocalAuthorityDistrict);

        return Result.Success(result);
    }

    private async Task<IResult<LocationFilterParameters>> GetSearchLocation(GetTuitionPartnerQuery request,
        CancellationToken cancellationToken)
    {
        var validationResults = await new Validator().ValidateAsync(request, cancellationToken);

        if (string.IsNullOrWhiteSpace(request.SearchModel?.Postcode))
            return Result.Success(new LocationFilterParameters { });

        return !validationResults.IsValid
            ? Result.Invalid<LocationFilterParameters>(validationResults.Errors)
            : (await _locationService.GetLocationFilterParametersAsync(request.SearchModel?.Postcode!)).TryValidate();
    }

    private async Task<IResult<TuitionPartnerResult>> FindTuitionPartner(LocationFilterParameters parameters,
        GetTuitionPartnerQuery request, CancellationToken cancellationToken)
    {
        int? id = null;
        if (int.TryParse(request.Id, out var parsedId))
        {
            id = parsedId;
        }
        else
        {
            var tuitionPartnersIds = await _tuitionPartnerService.GetTuitionPartnersFilteredAsync(
                new TuitionPartnersFilter
                {
                    LocalAuthorityDistrictId = parameters.LocalAuthorityDistrictId,
                    SeoUrls = new string[] { request.Id }
                }, cancellationToken);

            if (tuitionPartnersIds?.Length == 1)
            {
                id = tuitionPartnersIds[0];
            }
        }

        if (id == null)
        {
            //shouldn't get here, unless manually changed query string
            _logger.LogInformation("No TP found for the invalid Id '{Id}' provided", request.Id);
            return Result.Error<TuitionPartnerResult>();
        }

        var tuitionPartners = (await _tuitionPartnerService.GetTuitionPartnersAsync(new TuitionPartnerRequest
        {
            TuitionPartnerIds = new[] { id!.Value },
            LocalAuthorityDistrictId = parameters.LocalAuthorityDistrictId,
            Urn = parameters.Urn
        }, cancellationToken)).ToList();

        if (tuitionPartners.Count == 1) return Result.Success(tuitionPartners.First());

        _logger.LogWarning(
            "Did not return a single TP for the Id '{Id}' and LAD Id {LocalAuthorityDistrictId} provided.  {Count} results were returned",
            request.Id, parameters.LocalAuthorityDistrictId, tuitionPartners.Count);
        return Result.Error<TuitionPartnerResult>();
    }

    private async Task<LocalAuthorityDistrictCoverage[]> GetLocalAuthorityDistricts(
        GetTuitionPartnerQuery request, int tpId)
    {
        if (!request.ShowLocationsCovered && !request.ShowFullInfo) return Array.Empty<LocalAuthorityDistrictCoverage>();

        var coverage = await _db.LocalAuthorityDistrictCoverage
            .Where(e => e.TuitionPartnerId == tpId).ToArrayAsync();

        var coverageDictionary = coverage
            .GroupBy(e => e.TuitionSettingId)
            .ToDictionary(e => (TuitionSetting)e.Key, e => e.ToDictionary(x => x.LocalAuthorityDistrictId, x => x));

        var regions = await _db.Regions
            .Include(e => e.LocalAuthorityDistricts.OrderBy(x => x.Code))
            .OrderBy(e => e.Name)
            .ToDictionaryAsync(e => e, e => e.LocalAuthorityDistricts);

        var result = new Dictionary<int, LocalAuthorityDistrictCoverage>();

        foreach (var (region, lads) in regions)
        {
            foreach (var lad in lads)
            {
                var faceToFace = coverageDictionary.ContainsKey(Enums.TuitionSetting.FaceToFace) &&
                               coverageDictionary[Enums.TuitionSetting.FaceToFace].ContainsKey(lad.Id);
                var online = coverageDictionary.ContainsKey(Enums.TuitionSetting.Online) &&
                             coverageDictionary[Enums.TuitionSetting.Online].ContainsKey(lad.Id);
                result[lad.Id] = new LocalAuthorityDistrictCoverage(region.Name, lad.Code, lad.Name, faceToFace, online);
            }
        }

        return result.Values.ToArray();
    }

    private static Dictionary<int, GroupPrice> GetPricing(ICollection<Price> prices)
    {
        (Func<IEnumerable<Price>, decimal?> min, Func<IEnumerable<Price>, decimal?> max) online =
            (prices => MinPrice(prices, TuitionSetting.Online), prices => MaxPrice(prices, TuitionSetting.Online));

        (Func<IEnumerable<Price>, decimal?> min, Func<IEnumerable<Price>, decimal?> max) faceToFace =
            (prices => MinPrice(prices, TuitionSetting.FaceToFace), prices => MaxPrice(prices, TuitionSetting.FaceToFace));

        return prices
            .GroupBy(x => x.GroupSize)
            .ToDictionary(
                key => key.Key,
                value => new GroupPrice
                (faceToFace.min(value)
                    , faceToFace.max(value)
                    , online.min(value)
                    , online.max(value)
                )
            )
            .Where(x => x.Value.HasAtLeastOnePrice)
            .OrderBy(x => x.Key)
            .ToDictionary(k => k.Key, v => v.Value);

        static decimal? MinPrice(IEnumerable<Price> value, TuitionSetting tuitionSetting)
            => MinMaxPrice(value, tuitionSetting, Enumerable.MinBy);

        static decimal? MaxPrice(IEnumerable<Price> value, TuitionSetting tuitionSetting)
            => MinMaxPrice(value, tuitionSetting, Enumerable.MaxBy);

        static decimal? MinMaxPrice(
            IEnumerable<Price> value, TuitionSetting tuitionSetting,
            Func<IEnumerable<Price>, Func<Price, decimal>, Price?> minMax)
        {
            var pricesForTuition = value.Where(x => x.TuitionSetting.Id == (int)tuitionSetting);
            var minMaxForTuition = minMax(pricesForTuition, x => x.HourlyRate);
            return minMaxForTuition?.HourlyRate;
        }
    }

    private async
        Task<Dictionary<TuitionSetting, Dictionary<KeyStage, Dictionary<string, Dictionary<int, decimal>>>>>
        GetFullPricing(GetTuitionPartnerQuery request, ICollection<Price> prices)
    {
        if (!request.ShowFullPricing && !request.ShowFullInfo) return new();

        var fullPricing =
            new Dictionary<TuitionSetting,
                Dictionary<KeyStage, Dictionary<string, Dictionary<int, decimal>>>>();

        foreach (var tuitionSetting in new[] { TuitionSetting.FaceToFace, TuitionSetting.Online })
        {
            fullPricing[tuitionSetting] = new Dictionary<KeyStage, Dictionary<string, Dictionary<int, decimal>>>();
            foreach (var keyStage in new[]
                     {
                         KeyStage.KeyStage1, KeyStage.KeyStage2, KeyStage.KeyStage3,
                         KeyStage.KeyStage4
                     })
            {
                fullPricing[tuitionSetting][keyStage] = new Dictionary<string, Dictionary<int, decimal>>();

                var allKeyStageSubjects = await _lookupDataService.GetSubjectsAsync();

                var keyStageSubjects = allKeyStageSubjects.Where(e => e.KeyStageId == (int)keyStage)
                    .OrderBy(e => e.Name).ToArray();

                foreach (var subject in keyStageSubjects)
                    fullPricing[tuitionSetting][keyStage][subject.Name] = new Dictionary<int, decimal>();
            }
        }

        foreach (var price in prices)
        {
            var tuitionSetting = (TuitionSetting)price.TuitionSettingId;
            var keyStage = (KeyStage)price.Subject.KeyStageId;
            var subjectName = price.Subject.Name;

            fullPricing[tuitionSetting][keyStage][subjectName][price.GroupSize] = price.HourlyRate;
        }

        return fullPricing;
    }

    private sealed class Validator : AbstractValidator<GetTuitionPartnerQuery>
    {
        public Validator()
        {
            RuleFor(m => m.SearchModel!.Postcode)
                .Matches(StringConstants.PostcodeRegExp)
                .WithMessage("Enter a real postcode")
                .When(m => m.SearchModel != null && !string.IsNullOrEmpty(m.SearchModel?.Postcode));
        }
    }
}