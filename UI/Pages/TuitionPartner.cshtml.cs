using Application;
using Application.Constants;
using Application.Extensions;
using Domain;
using Domain.Constants;
using Domain.Search;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UI.Models;

namespace UI.Pages;

public class TuitionPartner : PageModel
{
    private readonly ILogger<TuitionPartner> _logger;
    private readonly IMediator _mediator;

    public TuitionPartner(ILogger<TuitionPartner> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public Command? Data { get; set; }

    public SearchModel? SearchModel { get; set; }

    public async Task<IActionResult> OnGetAsync(Query query)
    {
        SearchModel = new SearchModel(query);

        if (string.IsNullOrWhiteSpace(query.Id))
        {
            _logger.LogWarning("Null or whitespace id '{Id}' provided", query.Id);
            return NotFound();
        }

        var seoUrl = query.Id.ToSeoUrl();
        if (query.Id != seoUrl)
        {
            _logger.LogInformation("Non SEO id '{Id}' provided. Redirecting to {SeoUrl}", query.Id, seoUrl);
            return RedirectToPage((query with { Id = seoUrl }).ToRouteData());
        }

        Data = await _mediator.Send(query);

        if (Data == null)
        {
            return NotFound();
        }

        _logger.LogInformation("Tuition Partner {Name} found for id '{Id}'", Data.Name, query.Id);
        return Page();
    }

    public record Query(
            string Id,
            [FromQuery(Name = "show-locations-covered")]
            bool ShowLocationsCovered = false,
            [FromQuery(Name = "show-full-pricing")]
            bool ShowFullPricing = false)
        : SearchModel, IRequest<Command?>
    {
        public Dictionary<string, string> ToRouteData()
        {
            var dictionary = new Dictionary<string, string>
            {
                [nameof(Id)] = Id
            };

            if (ShowLocationsCovered)
            {
                dictionary["show-locations-covered"] = "true";
            }

            if (ShowFullPricing)
            {
                dictionary["show-full-pricing"] = "true";
            }

            return dictionary;
        }
    }

    public record Command(
        string Id, string Name, bool HasLogo, string Description, string[] Subjects,
        string[] TuitionTypes, string[] Ratios, Dictionary<int, GroupPrice> Prices,
        string Website, string PhoneNumber, string EmailAddress, string[] Address, bool HasSenProvision, bool IsVatCharged,
        LocalAuthorityDistrictCoverage[] LocalAuthorityDistricts,
        Dictionary<Enums.TuitionType, Dictionary<Enums.KeyStage, Dictionary<string, Dictionary<int, decimal>>>> AllPrices, string LegalStatus)
    {
        public bool HasPricingVariation => Prices.Any(x => x.Value.HasVariation);
    }

    public record struct GroupPrice(decimal? SchoolMin, decimal? SchoolMax, decimal? OnlineMin, decimal? OnlineMax)
    {
        public bool HasAtLeastOnePrice =>
            OnlineMin != null || OnlineMax != null || SchoolMin != null || SchoolMax != null;

        public bool HasVariation =>
            (OnlineMin != OnlineMax) || (SchoolMin != SchoolMax);
    }

    public record struct LocalAuthorityDistrictCoverage(string Region, string Code, string Name, bool InSchool, bool Online);

    private class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(m => m.Postcode)
                .Matches(StringConstants.PostcodeRegExp)
                .WithMessage("Enter a valid postcode")
                .When(m => !string.IsNullOrEmpty(m.Postcode));
        }
    }

    public class QueryHandler : IRequestHandler<Query, Command?>
    {
        private readonly ILocationFilterService _locationService;
        private readonly ITuitionPartnerService _tuitionPartnerService;
        private readonly INtpDbContext _db;
        private readonly ILogger<TuitionPartner> _logger;

        public QueryHandler(ILocationFilterService locationService, ITuitionPartnerService tuitionPartnerService, INtpDbContext db, ILogger<TuitionPartner> logger)
        {
            _locationService = locationService;
            _tuitionPartnerService = tuitionPartnerService;
            _db = db;
            _logger = logger;
        }

        public async Task<Command?> Handle(Query request, CancellationToken cancellationToken)
        {
            var tpResult = GetTPResult(request, cancellationToken);

            if (!tpResult.Result.IsSuccess) return null;

            var tp = tpResult.Result.Data.FirstResult;

            var subjects = tp.SubjectsCoverage.Select(x => x.Subject).Distinct().GroupBy(x => x.KeyStageId).Select(x => $"{((Enums.KeyStage)x.Key).DisplayName()} - {x.DisplayList()}");
            var types = tp.TuitionTypes.Select(x => x.Name).Distinct();
            var ratios = tp.Prices.Select(x => x.GroupSize).Distinct().Select(x => $"1 to {x}");
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
                tp.HasSenProvision,
                tp.IsVatCharged,
                lads,
                allPrices,
                tp.LegalStatus);
        }

        private async Task<IResult<TuitionPartnersResult>> GetTPResult(Query request, CancellationToken cancellationToken)
        {
            var locationResult = await GetSearchLocation(request, cancellationToken);

            LocationFilterParameters location = new();

            if (!locationResult.IsSuccess)
            {
                //Shouldn't be invalid, unless query string edited - since postcode on this page comes from previous validation
                _logger.LogWarning("Invalid postcode '{Postcode}' provided on TP details page", request.Postcode);

                //Set to null and contine to get nationwide data
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
            {
                return tpError.Cast<TuitionPartnersResult>();
            }

            var result = new TuitionPartnersResult(tpResult.Data, location.LocalAuthority);

            return Result.Success(result);
        }

        private async Task<IResult<LocationFilterParameters>> GetSearchLocation(Query request, CancellationToken cancellationToken)
        {
            var validationResults = await new Validator().ValidateAsync(request, cancellationToken);

            if (string.IsNullOrWhiteSpace(request.Postcode))
                return Result.Success(new LocationFilterParameters { });

            if (!validationResults.IsValid)
            {
                return Result.Invalid<LocationFilterParameters>(validationResults.Errors);
            }
            else
            {
                return (await _locationService.GetLocationFilterParametersAsync(request.Postcode!)).TryValidate();
            }
        }

        private async Task<IResult<TuitionPartnerResult>> FindTuitionPartner(
            LocationFilterParameters parameters,
            Query request,
            CancellationToken cancellationToken)
        {
            int? id = null;
            if (int.TryParse(request.Id, out var parsedId))
            {
                id = parsedId;
            }
            else
            {
                var tuitionPartnersIds = await _tuitionPartnerService.GetTuitionPartnersFilteredAsync(new TuitionPartnersFilter
                {
                    LocalAuthorityDistrictId = parameters.LocalAuthorityDistrictId,
                    SeoUrl = request.Id
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

            var tuitionPartners = await _tuitionPartnerService.GetTuitionPartnersAsync(new TuitionPartnerRequest
            {
                TuitionPartnerIds = new int[] { id!.Value },
                LocalAuthorityDistrictId = parameters.LocalAuthorityDistrictId,
                Urn = parameters.Urn
            }, cancellationToken);

            if (tuitionPartners?.Count() != 1)
            {
                _logger.LogWarning("Did not return a single TP for the Id '{Id}' and LAD Id {LocalAuthorityDistrictId} provided.  {Count} results were returned", request.Id, parameters.LocalAuthorityDistrictId, tuitionPartners?.Count());
                return Result.Error<TuitionPartnerResult>();
            }

            return Result.Success(tuitionPartners.First());
        }

        private async Task<LocalAuthorityDistrictCoverage[]> GetLocalAuthorityDistricts(Query request, int tpId)
        {
            if (!request.ShowLocationsCovered) return Array.Empty<LocalAuthorityDistrictCoverage>();

            var coverage = await _db.LocalAuthorityDistrictCoverage.Where(e => e.TuitionPartnerId == tpId)
                .ToArrayAsync();

            var coverageDictionary = coverage
                .GroupBy(e => e.TuitionTypeId)
                .ToDictionary(e => (Enums.TuitionType)e.Key, e => e.ToDictionary(x => x.LocalAuthorityDistrictId, x => x));

            var regions = await _db.Regions
                .Include(e => e.LocalAuthorityDistricts.OrderBy(x => x.Code))
                .OrderBy(e => e.Name)
                .ToDictionaryAsync(e => e, e => e.LocalAuthorityDistricts);

            var result = new Dictionary<int, LocalAuthorityDistrictCoverage>();

            foreach (var (region, lads) in regions)
            {
                foreach (var lad in lads)
                {
                    var inSchool = coverageDictionary.ContainsKey(Enums.TuitionType.InSchool) && coverageDictionary[Enums.TuitionType.InSchool].ContainsKey(lad.Id);
                    var online = coverageDictionary.ContainsKey(Enums.TuitionType.Online) && coverageDictionary[Enums.TuitionType.Online].ContainsKey(lad.Id);
                    result[lad.Id] = new LocalAuthorityDistrictCoverage(region.Name, lad.Code, lad.Name, inSchool, online);
                }
            }

            return result.Values.ToArray();
        }

        private static Dictionary<int, GroupPrice> GetPricing(ICollection<Price> prices)
        {
            (Func<IEnumerable<Price>, decimal?> min, Func<IEnumerable<Price>, decimal?> max) online =
                (prices => MinPrice(prices, TuitionTypes.Online), prices => MaxPrice(prices, TuitionTypes.Online));

            (Func<IEnumerable<Price>, decimal?> min, Func<IEnumerable<Price>, decimal?> max) inSchool =
                (prices => MinPrice(prices, TuitionTypes.InSchool), prices => MaxPrice(prices, TuitionTypes.InSchool));

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

            static decimal? MinPrice(IEnumerable<Price> value, TuitionTypes tuitionType)
                => MinMaxPrice(value, tuitionType, Enumerable.MinBy);

            static decimal? MaxPrice(IEnumerable<Price> value, TuitionTypes tuitionType)
                => MinMaxPrice(value, tuitionType, Enumerable.MaxBy);

            static decimal? MinMaxPrice(
                IEnumerable<Price> value, TuitionTypes tuitionType,
                Func<IEnumerable<Price>, Func<Price, decimal>, Price?> minMax)
            {
                var pricesForTuition = value.Where(x => x.TuitionType.Id == (int)tuitionType);
                var minMaxForTuition = minMax(pricesForTuition, x => x.HourlyRate);
                return minMaxForTuition?.HourlyRate;
            }
        }

        private async Task<Dictionary<Enums.TuitionType, Dictionary<Enums.KeyStage, Dictionary<string, Dictionary<int, decimal>>>>> GetFullPricing(Query request, ICollection<Price> prices)
        {
            if (!request.ShowFullPricing) return new();

            var fullPricing = new Dictionary<Enums.TuitionType, Dictionary<Enums.KeyStage, Dictionary<string, Dictionary<int, decimal>>>>();

            foreach (var tuitionType in new[] { Enums.TuitionType.InSchool, Enums.TuitionType.Online })
            {
                fullPricing[tuitionType] = new Dictionary<Enums.KeyStage, Dictionary<string, Dictionary<int, decimal>>>();
                foreach (var keyStage in new[] { Enums.KeyStage.KeyStage1, Enums.KeyStage.KeyStage2, Enums.KeyStage.KeyStage3, Enums.KeyStage.KeyStage4 })
                {
                    fullPricing[tuitionType][keyStage] = new Dictionary<string, Dictionary<int, decimal>>();

                    var keyStageSubjects = await _db.Subjects.Where(e => e.KeyStageId == (int)keyStage).OrderBy(e => e.Name).ToArrayAsync();
                    foreach (var subject in keyStageSubjects)
                    {
                        fullPricing[tuitionType][keyStage][subject.Name] = new Dictionary<int, decimal>();
                    }
                }
            }

            foreach (var price in prices)
            {
                var tuitionType = (Enums.TuitionType)price.TuitionTypeId;
                var keyStage = (Enums.KeyStage)price.Subject.KeyStageId;
                var subjectName = price.Subject.Name;

                fullPricing[tuitionType][keyStage][subjectName][price.GroupSize] = price.HourlyRate;
            }

            return fullPricing;
        }
    }
}