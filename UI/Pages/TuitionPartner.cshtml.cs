using Application.Extensions;
using Domain;
using Domain.Constants;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UI.Extensions;

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

    public SearchModel? AllSearchData { get; set; }

    public async Task<IActionResult> OnGetAsync(Query query)
    {
        AllSearchData = TempData.Peek<SearchModel>("AllSearchData");

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

        Data = await _mediator.Send(query with
        {
            LocalAuthorityDistrictCode = TempData.Peek<string>("LocalAuthorityDistrictCode")
        });

        if (Data == null)
        {
            _logger.LogInformation("No Tuition Partner found for id '{Id}'", query.Id);
            return NotFound();
        }

        _logger.LogInformation("Tuition Partner {Name} found for id '{Id}'", Data.Name, query.Id);
        return Page();
    }

    public record Query(
            string Id,
            string? LocalAuthorityDistrictCode = null,
            [FromQuery(Name = "show-locations-covered")]
            bool ShowLocationsCovered = false,
            [FromQuery(Name = "show-full-pricing")]
            bool ShowFullPricing = false)
        : IRequest<Command?>
    {
        public Dictionary<string, string> ToRouteData()
        {
            var dictionary = new Dictionary<string, string>();

            dictionary[nameof(Id)] = Id;

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
        string Name, string Description, string[] Subjects,
        string[] TuitionTypes, string[] Ratios, Dictionary<int, GroupPrice> Prices,
        string Website, string PhoneNumber, string EmailAddress, string Address, bool HasSenProvision,
        LocalAuthorityDistrictCoverage[] LocalAuthorityDistricts,
        Dictionary<TuitionType, Dictionary<KeyStage, Dictionary<string, Dictionary<int, decimal>>>> AllPrices);

    public record struct GroupPrice(decimal? SchoolMin, decimal? SchoolMax, decimal? OnlineMin, decimal? OnlineMax)
    {
        public bool HasAtLeastOnePrice =>
            OnlineMin != null || OnlineMax != null || SchoolMin != null || SchoolMax != null;
    }

    public record struct LocalAuthorityDistrictCoverage(string Region, string Code, string Name, bool InSchool, bool Online);

    public class QueryHandler : IRequestHandler<Query, Command?>
    {
        private readonly NtpDbContext _db;

        public QueryHandler(NtpDbContext db) => _db = db;

        public async Task<Command?> Handle(Query request, CancellationToken cancellationToken)
        {
            var queryable = _db.TuitionPartners
                .Include(e => e.SubjectCoverage)
                .ThenInclude(e => e.Subject)
                .Include(x => x.Prices)
                .ThenInclude(x => x.TuitionType)
                .Include(x => x.Prices)
                .ThenInclude(x => x.Subject)
                .ThenInclude(x => x.KeyStage);

            Domain.TuitionPartner? tp;

            if (int.TryParse(request.Id, out var id))
            {
                tp = await queryable.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            }
            else
            {
                tp = await queryable.FirstOrDefaultAsync(x => x.SeoUrl == request.Id, cancellationToken);
            }

            if (tp == null) return null;

            var subjects = tp.SubjectCoverage.Select(x => x.Subject).Distinct().GroupBy(x => x.KeyStageId).Select(x => $"{((KeyStage)x.Key).DisplayName()} - {x.DisplayList()}");
            var types = await GetTuitionTypesCovered(request.LocalAuthorityDistrictCode, tp, cancellationToken);
            var ratios = tp.Prices.Select(x => x.GroupSize).Distinct().Select(x => $"1 to {x}");
            var prices = GetPricing(types, tp.Prices);

            var lads = await GetLocalAuthorityDistricts(request, tp.Id);

            var allPrices = await GetFullPricing(request, tp.Prices);

            return new(
                tp.Name,
                tp.Description,
                subjects.ToArray(),
                types.ToArray(),
                ratios.ToArray(),
                prices,
                tp.Website,
                tp.PhoneNumber,
                tp.Email,
                tp.Address,
                tp.HasSenProvision,
                lads,
                allPrices
                );
        }

        private async Task<IEnumerable<string>> GetTuitionTypesCovered(
            string? localAuthorityDistrictCode,
            Domain.TuitionPartner tp,
            CancellationToken cancellationToken)
        {
            var coverageQuery = _db
                .LocalAuthorityDistrictCoverage
                .Where(e => e.TuitionPartnerId == tp.Id);

            if (localAuthorityDistrictCode != null)
            {
                coverageQuery = coverageQuery
                    .Where(e => e.LocalAuthorityDistrict.Code == localAuthorityDistrictCode);
            }

            var types = await coverageQuery
                .Select(x => x.TuitionType.Name)
                .Distinct()
                .ToArrayAsync(cancellationToken);

            return types;
        }

        private async Task<LocalAuthorityDistrictCoverage[]> GetLocalAuthorityDistricts(Query request, int tpId)
        {
            if (!request.ShowLocationsCovered) return Array.Empty<LocalAuthorityDistrictCoverage>();

            var coverage = await _db.LocalAuthorityDistrictCoverage.Where(e => e.TuitionPartnerId == tpId)
                .ToArrayAsync();

            var coverageDictionary = coverage
                .GroupBy(e => e.TuitionTypeId)
                .ToDictionary(e => (TuitionType)e.Key, e => e.ToDictionary(x => x.LocalAuthorityDistrictId, x => x));

            var regions = await _db.Regions
                .Include(e => e.LocalAuthorityDistricts.OrderBy(x => x.Code))
                .OrderBy(e => e.Name)
                .ToDictionaryAsync(e => e, e => e.LocalAuthorityDistricts);

            var result = new Dictionary<int, LocalAuthorityDistrictCoverage>();

            foreach (var (region, lads) in regions)
            {
                foreach (var lad in lads)
                {
                    var inSchool = coverageDictionary.ContainsKey(TuitionType.InSchool) && coverageDictionary[TuitionType.InSchool].ContainsKey(lad.Id);
                    var online = coverageDictionary.ContainsKey(TuitionType.Online) && coverageDictionary[TuitionType.Online].ContainsKey(lad.Id);
                    result[lad.Id] = new LocalAuthorityDistrictCoverage(region.Name, lad.Code, lad.Name, inSchool, online);
                }
            }

            return result.Values.ToArray();
        }

        private static Dictionary<int, GroupPrice> GetPricing(IEnumerable<string> types, ICollection<Price> prices)
        {
            (Func<IEnumerable<Price>, decimal?> min, Func<IEnumerable<Price>, decimal?> max) online =
                types.Contains("Online")
                ? (prices => MinPrice(prices, TuitionTypes.Online), prices => MaxPrice(prices, TuitionTypes.Online))
                : (prices => null, prices => null);

            (Func<IEnumerable<Price>, decimal?> min, Func<IEnumerable<Price>, decimal?> max) inSchool =
                types.Contains("In School")
                ? (prices => MinPrice(prices, TuitionTypes.InSchool), prices => MaxPrice(prices, TuitionTypes.InSchool))
                : (prices => null, prices => null);

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

        private async Task<Dictionary<TuitionType, Dictionary<KeyStage, Dictionary<string, Dictionary<int, decimal>>>>> GetFullPricing(Query request, ICollection<Price> prices)
        {
            if (!request.ShowFullPricing) return new();

            var fullPricing = new Dictionary<TuitionType, Dictionary<KeyStage, Dictionary<string, Dictionary<int, decimal>>>>();

            foreach (var tuitionType in new[] { TuitionType.InSchool, TuitionType.Online })
            {
                fullPricing[tuitionType] = new Dictionary<KeyStage, Dictionary<string, Dictionary<int, decimal>>>();
                foreach (var keyStage in new[] { KeyStage.KeyStage1, KeyStage.KeyStage2, KeyStage.KeyStage3, KeyStage.KeyStage4 })
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
                var tuitionType = (TuitionType)price.TuitionTypeId;
                var keyStage = (KeyStage)price.Subject.KeyStageId;
                var subjectName = price.Subject.Name;

                fullPricing[tuitionType][keyStage][subjectName][price.GroupSize] = price.HourlyRate;
            }

            return fullPricing;
        }
    }
}