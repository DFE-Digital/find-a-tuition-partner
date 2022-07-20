using Application.Extensions;
using Domain;
using Domain.Constants;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IActionResult> OnGetAsync(Query query)
    {
        if (string.IsNullOrWhiteSpace(query.Id))
        {
            _logger.LogWarning("Null or whitespace id '{Id}' provided", query.Id);
            return NotFound();
        }

        var seoUrl = query.Id.ToSeoUrl();
        if (query.Id != seoUrl)
        {
            _logger.LogInformation("Non SEO id '{Id}' provided. Redirecting to {SeoUrl}", query.Id, seoUrl);
            return RedirectToPage(new { Id = seoUrl });
        }

        Data = await _mediator.Send(query);

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
        [FromQuery(Name = "show-locations-covered")]
        bool ShowLocationsCovered = false,
        [FromQuery(Name = "show-full-pricing")]
        bool ShowFullPricing = false)
        : IRequest<Command?>;

    public record Command(
        string Name, string Description, string[] Subjects,
        string[] TuitionTypes, string[] Ratios, Dictionary<int, GroupPrice> Prices,
        string Website, string PhoneNumber, string EmailAddress, string Address,
        Dictionary<Domain.TuitionType, string[]> LocalAuthorityDistricts,
        Dictionary<TuitionType, Dictionary<KeyStage, Dictionary<string, Dictionary<int, decimal>>>> AllPrices);

    public record struct GroupPrice(decimal? SchoolMin, decimal? SchoolMax, decimal? OnlineMin, decimal? OnlineMax);

    public record SubjectPrice(string Subject, decimal SchoolPrice, decimal OnlinePrice);

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
            var types = tp.Prices.Select(x => x.TuitionType.Name).Distinct();
            var ratios = tp.Prices.Select(x => x.GroupSize).Distinct().Select(x => $"1 to {x}");
            var prices = GetPricing(tp.Prices);

            var lads = GetLocalAuthorityDistricts(request, tp.Id);

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
                lads,
                allPrices
                );
        }

        private Dictionary<Domain.TuitionType, string[]> GetLocalAuthorityDistricts(Query request, int tpId)
        {
            if (!request.ShowLocationsCovered) return new();

            return _db.LocalAuthorityDistrictCoverage
                .Include(x => x.LocalAuthorityDistrict)
                .Where(x => x.TuitionPartnerId == tpId)
                .Where(cov => cov.LocalAuthorityDistrict != null
                           && cov.LocalAuthorityDistrict.Name != null)
                .AsEnumerable()
                .GroupBy(x => x.TuitionType)
                .ToDictionary(
                    key => key.Key,
                    value => value
                        .Select(cov => cov.LocalAuthorityDistrict.Name)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToArray());
        }

        private static Dictionary<int, GroupPrice> GetPricing(ICollection<Price> prices)
        {
            return prices
                .GroupBy(x => x.GroupSize)
                .ToDictionary(
                    key => key.Key,
                    value => new GroupPrice
                            ( MinPrice(value, TuitionTypes.InSchool)
                            , MaxPrice(value, TuitionTypes.InSchool)
                            , MinPrice(value, TuitionTypes.Online)
                            , MaxPrice(value, TuitionTypes.Online)
                            )
                    );

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
