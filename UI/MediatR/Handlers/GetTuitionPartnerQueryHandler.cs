using Domain;
using Domain.Search;
using LocalAuthorityDistrictCoverage = UI.Structs.LocalAuthorityDistrictCoverage;

namespace UI.MediatR.Handlers;

public class GetTuitionPartnerQueryHandler : IRequestHandler<GetTuitionPartnerQuery, TuitionPartnerModel?>
{
    private readonly ILocationFilterService _locationService;
    private readonly ITuitionPartnerService _tuitionPartnerService;
    private readonly INtpDbContext _db;
    private readonly ILogger<TuitionPartner> _logger;

    public GetTuitionPartnerQueryHandler(ILocationFilterService locationService,
        ITuitionPartnerService tuitionPartnerService, INtpDbContext db, ILogger<TuitionPartner> logger)
    {
        _locationService = locationService;
        _tuitionPartnerService = tuitionPartnerService;
        _db = db;
        _logger = logger;
    }

    public async Task<TuitionPartnerModel?> Handle(GetTuitionPartnerQuery request, CancellationToken cancellationToken)
    {
        var tpResult = GetTpResult(request, cancellationToken);

        if (!tpResult.Result.IsSuccess) return null;

        var tp = tpResult.Result.Data.FirstResult;

        var subjects = tp.SubjectsCoverage.Select(x => x.Subject).Distinct().GroupBy(x => x.KeyStageId)
            .Select(x => $"{((Domain.Enums.KeyStage)x.Key).DisplayName()} - {x.DisplayList()}");
        var types = tp.TuitionTypes.Select(x => x.Name).Distinct();
        var ratios = tp.Prices.Select(x => x.GroupSize).Distinct().Select(x => $"{((Domain.Enums.GroupSize)x).DisplayName()}");
        var prices = GetPricing(tp.Prices);
        var lads = await GetLocalAuthorityDistricts(request, tp.Id);
        var allPrices = await GetFullPricing(request, tp.Prices);

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
            tpResult.Result.Data.LocalAuthorityName);
    }

    private async Task<IResult<TuitionPartnersResult>> GetTpResult(GetTuitionPartnerQuery request,
        CancellationToken cancellationToken)
    {
        var locationResult = await GetSearchLocation(request, cancellationToken);

        LocationFilterParameters location = new();

        if (!locationResult.IsSuccess)
        {
            //Shouldn't be invalid, unless query string edited - since postcode on this page comes from previous page with validation
            _logger.LogWarning("Invalid postcode '{Postcode}' provided on TP details page", request.Postcode);

            //Set to null and continue to get nationwide data
            request.Postcode = null;
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

        var result = new TuitionPartnersResult(tpResult.Data, location.LocalAuthority);

        return Result.Success(result);
    }

    private async Task<IResult<LocationFilterParameters>> GetSearchLocation(GetTuitionPartnerQuery request,
        CancellationToken cancellationToken)
    {
        var validationResults = await new Validator().ValidateAsync(request, cancellationToken);

        if (string.IsNullOrWhiteSpace(request.Postcode))
            return Result.Success(new LocationFilterParameters { });

        return !validationResults.IsValid
            ? Result.Invalid<LocationFilterParameters>(validationResults.Errors)
            : (await _locationService.GetLocationFilterParametersAsync(request.Postcode!)).TryValidate();
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
            _logger.LogWarning("No TP found for the invalid Id '{Id}' provided", request.Id);
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
        if (!request.ShowLocationsCovered) return Array.Empty<LocalAuthorityDistrictCoverage>();

        var coverage = await _db.LocalAuthorityDistrictCoverage
            .Where(e => e.TuitionPartnerId == tpId).ToArrayAsync();

        var coverageDictionary = coverage
            .GroupBy(e => e.TuitionTypeId)
            .ToDictionary(e => (Domain.Enums.TuitionType)e.Key, e => e.ToDictionary(x => x.LocalAuthorityDistrictId, x => x));

        var regions = await _db.Regions
            .Include(e => e.LocalAuthorityDistricts.OrderBy(x => x.Code))
            .OrderBy(e => e.Name)
            .ToDictionaryAsync(e => e, e => e.LocalAuthorityDistricts);

        var result = new Dictionary<int, LocalAuthorityDistrictCoverage>();

        foreach (var (region, lads) in regions)
        {
            foreach (var lad in lads)
            {
                var inSchool = coverageDictionary.ContainsKey(Domain.Enums.TuitionType.InSchool) &&
                               coverageDictionary[Domain.Enums.TuitionType.InSchool].ContainsKey(lad.Id);
                var online = coverageDictionary.ContainsKey(Domain.Enums.TuitionType.Online) &&
                             coverageDictionary[Domain.Enums.TuitionType.Online].ContainsKey(lad.Id);
                result[lad.Id] = new LocalAuthorityDistrictCoverage(region.Name, lad.Code, lad.Name, inSchool, online);
            }
        }

        return result.Values.ToArray();
    }

    private static Dictionary<int, GroupPrice> GetPricing(ICollection<Price> prices)
    {
        (Func<IEnumerable<Price>, decimal?> min, Func<IEnumerable<Price>, decimal?> max) online =
            (prices => MinPrice(prices, Domain.Enums.TuitionType.Online), prices => MaxPrice(prices, Domain.Enums.TuitionType.Online));

        (Func<IEnumerable<Price>, decimal?> min, Func<IEnumerable<Price>, decimal?> max) inSchool =
            (prices => MinPrice(prices, Domain.Enums.TuitionType.InSchool), prices => MaxPrice(prices, Domain.Enums.TuitionType.InSchool));

        return prices
            .GroupBy(x => x.GroupSize)
            .ToDictionary(
                key => key.Key,
                value => new GroupPrice
                (inSchool.min(value)
                    , inSchool.max(value)
                    , online.min(value)
                    , online.max(value)
                )
            )
            .Where(x => x.Value.HasAtLeastOnePrice)
            .ToDictionary(k => k.Key, v => v.Value);

        static decimal? MinPrice(IEnumerable<Price> value, Domain.Enums.TuitionType tuitionType)
            => MinMaxPrice(value, tuitionType, Enumerable.MinBy);

        static decimal? MaxPrice(IEnumerable<Price> value, Domain.Enums.TuitionType tuitionType)
            => MinMaxPrice(value, tuitionType, Enumerable.MaxBy);

        static decimal? MinMaxPrice(
            IEnumerable<Price> value, Domain.Enums.TuitionType tuitionType,
            Func<IEnumerable<Price>, Func<Price, decimal>, Price?> minMax)
        {
            var pricesForTuition = value.Where(x => x.TuitionType.Id == (int)tuitionType);
            var minMaxForTuition = minMax(pricesForTuition, x => x.HourlyRate);
            return minMaxForTuition?.HourlyRate;
        }
    }

    private async
        Task<Dictionary<Domain.Enums.TuitionType, Dictionary<Domain.Enums.KeyStage, Dictionary<string, Dictionary<int, decimal>>>>>
        GetFullPricing(GetTuitionPartnerQuery request, ICollection<Price> prices)
    {
        if (!request.ShowFullPricing) return new();

        var fullPricing =
            new Dictionary<Domain.Enums.TuitionType,
                Dictionary<Domain.Enums.KeyStage, Dictionary<string, Dictionary<int, decimal>>>>();

        foreach (var tuitionType in new[] { Domain.Enums.TuitionType.InSchool, Domain.Enums.TuitionType.Online })
        {
            fullPricing[tuitionType] = new Dictionary<Domain.Enums.KeyStage, Dictionary<string, Dictionary<int, decimal>>>();
            foreach (var keyStage in new[]
                     {
                         Domain.Enums.KeyStage.KeyStage1, Domain.Enums.KeyStage.KeyStage2, Domain.Enums.KeyStage.KeyStage3,
                         Domain.Enums.KeyStage.KeyStage4
                     })
            {
                fullPricing[tuitionType][keyStage] = new Dictionary<string, Dictionary<int, decimal>>();

                var keyStageSubjects = await _db.Subjects.Where(e => e.KeyStageId == (int)keyStage)
                    .OrderBy(e => e.Name).ToArrayAsync();
                foreach (var subject in keyStageSubjects)
                    fullPricing[tuitionType][keyStage][subject.Name] = new Dictionary<int, decimal>();
            }
        }

        foreach (var price in prices)
        {
            var tuitionType = (Domain.Enums.TuitionType)price.TuitionTypeId;
            var keyStage = (Domain.Enums.KeyStage)price.Subject.KeyStageId;
            var subjectName = price.Subject.Name;

            fullPricing[tuitionType][keyStage][subjectName][price.GroupSize] = price.HourlyRate;
        }

        return fullPricing;
    }

    private class Validator : AbstractValidator<GetTuitionPartnerQuery>
    {
        public Validator()
        {
            RuleFor(m => m.Postcode)
                .Matches(StringConstants.PostcodeRegExp)
                .WithMessage("Enter a valid postcode")
                .When(m => !string.IsNullOrEmpty(m.Postcode));
        }
    }
}