using Application.Extensions;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace UI.Pages.FindATuitionPartner;

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
            _logger.LogWarning($"Null or whitespace id '{query.Id}' provided");
            return NotFound();
        }

        var seoUrl = query.Id.ToSeoUrl();
        if (query.Id != seoUrl)
        {
            _logger.LogInformation($"Non SEO id '{query.Id}' provided. Redirecting to {seoUrl}");
            return RedirectToPage(nameof(TuitionPartner), new { Id = seoUrl });
        }

        Data = await _mediator.Send(query);

        if (Data == null)
        {
            _logger.LogInformation($"No Tuition Partner found for id '{query.Id}'");
            return NotFound();
        }

        _logger.LogInformation($"Tuition Partner {Data.Name} found for id '{query.Id}'");
        return Page();
    }

    public record Query(string Id) : IRequest<Command>;

    public record Command(
        string Name, string Description, string[] Subjects,
        string[] TuitionTypes, string[] Ratios, SubjectPrice[] Prices,
        string Website, string PhoneNumber, string EmailAddress);

    public record SubjectPrice(string Subject, decimal Price);

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
            var prices = tp.Prices.Where(x => x.GroupSize == 3).GroupBy(x => x.Subject).Select(x => 
                new SubjectPrice($"{x.Key.KeyStage.Name} - {x.Key.Name}", x.MaxBy(y => y.HourlyRate)?.HourlyRate ?? 0));

            return new(
                tp.Name,
                tp.Description,
                subjects.ToArray(),
                types.ToArray(),
                ratios.ToArray(),
                prices.ToArray(),
                tp.Website,
                tp.PhoneNumber,
                tp.Email
                );
        }
    }
}
