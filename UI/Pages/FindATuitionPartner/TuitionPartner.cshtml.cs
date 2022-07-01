using Domain.Constants;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace UI.Pages.FindATuitionPartner;

public class TuitionPartner : PageModel
{
    private readonly IMediator _mediator;

    public TuitionPartner(IMediator mediator) => _mediator = mediator;

    public Command? Data { get; set; }

    public async Task OnGetAsync(Query query)
    {
        Data = await _mediator.Send(query);
    }

    public record Query(int Id) : IRequest<Command>;

    public record Command(
        string Name, string Description, string[] Subjects,
        string[] TuitionTypes, string[] Ratios, SubjectPrice[] Prices,
        string Website, string PhoneNumber, string EmailAddress);

    public record SubjectPrice(string Subject, int Price);

    public class QueryHandler : IRequestHandler<Query, Command>
    {
        private readonly NtpDbContext _db;

        public QueryHandler(NtpDbContext db) => _db = db;

        public async Task<Command> Handle(Query request, CancellationToken cancellationToken)
        {
            var tp = await _db.TuitionPartners
                .Include(x => x.Prices)
                .ThenInclude(x => x.TuitionType)
                .Include(x => x.Prices)
                .ThenInclude(x => x.Subject)
                .ThenInclude(x => x.KeyStage)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            var subjects = tp.Prices.Select(x => x.Subject.Name).Distinct();
            var types = tp.Prices.Select(x => x.TuitionType.Name).Distinct();
            var ratios = tp.Prices.Select(x => x.GroupSize).Distinct().Select(x => $"1 to {x}");
            var prices = tp.Prices.GroupBy(x => x.Subject).Select(x => 
                new SubjectPrice($"{x.Key.KeyStage.Name} - {x.Key.Name}", (int)(x.MaxBy(y => y.HourlyRate)?.HourlyRate ?? 0)));

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
