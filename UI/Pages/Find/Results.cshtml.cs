using Application;
using Application.Handlers;
using Domain.Search;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace UI.Pages.Find;

public class Results : PageModel
{
    private readonly IMediator mediator;

    public Results(IMediator mediator) => this.mediator = mediator;

    [BindProperty(SupportsGet = true)]
    public Command Data { get; set; } = new();

    public async Task OnGet()
    {
        if (!ModelState.IsValid) return; 
        Data = await mediator.Send(Data);
    }

    public record Query : SearchModel, IRequest<Command>
    {
        public Query() { }
        public Query(SearchModel query) : base(query) { }
    }

    public record Command : SearchModel, IRequest<Command>
    {
        public Command() { }
        public Command(SearchModel query) : base(query) { }
        public IEnumerable<Selectable> AllSubjects { get; set; } = new List<Selectable>();
        public IEnumerable<Selectable> AllTuitionTypes { get; set; } = new List<Selectable>();

        public TuitionPartnerSearchResultsPage? Results { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(m => m.Postcode)
                .NotEmpty()
                .WithMessage("Enter a postcode");

            RuleFor(m => m.Postcode)
                .Matches(@"[a-zA-Z]{1,2}([0-9]{1,2}|[0-9][a-zA-Z])\s*[0-9][a-zA-Z]{2}")
                .WithMessage("Enter a valid postcode");

            RuleFor(m => m.Subjects)
                .NotEmpty()
                .WithMessage("Select the subject or subjects");
        }
    }

    public class Handler : IRequestHandler<Command, Command>
    {
        private readonly ILocationFilterService locationService;
        private readonly INtpDbContext db;
        private readonly IMediator mediator;

        public Handler(ILocationFilterService locationService, INtpDbContext db, IMediator mediator)
        {
            this.locationService = locationService;
            this.db = db;
            this.mediator = mediator;
        }

        public async Task<Command> Handle(Command request, CancellationToken cancellationToken)
        {
            //if (request.Postcode == null) throw new FluentValidation.ValidationException("Enter a postcode");
            var loc = await locationService.GetLocationFilterParametersAsync(request.Postcode);
            var subjects = await db.Subjects.Where(s => request.Subjects.Contains(s.Name)).ToListAsync(cancellationToken);

            var allSubjects = await mediator.Send(new Subjects.Query { Subjects = request.Subjects }, cancellationToken);

            var cmd = new SearchTuitionPartnerHandler.Command
            {
                OrderBy = TuitionPartnerOrderBy.Name,
                LocalAuthorityDistrictCode = loc.LocalAuthorityDistrictCode,
                SubjectIds = subjects.Select(x => x.Id),
            };

            return new(request)
            {
                AllSubjects = allSubjects.AllSubjects,
                Results = await mediator.Send(cmd, cancellationToken),
            };
        }
    }
}