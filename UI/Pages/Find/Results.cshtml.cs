using Application;
using Application.Handlers;
using Domain.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace UI.Pages.Find;

public class Results : PageModel
{
    private readonly IMediator mediator;

    public Results(IMediator mediator) => this.mediator = mediator;

    [BindProperty]
    public Command Data { get; set; } = new();

    public async Task OnGet(Query query)
    {
        Data = await mediator.Send(query);
    }

    //public void OnPost()
    //{
    //}

    public record Query : SearchModel, IRequest<Command>
    {
        public Query() { }
        public Query(SearchModel query) : base(query) { }
    }

    public record Command : SearchModel, IRequest<SearchModel>
    {
        public Command() { }
        public Command(SearchModel query) : base(query) { }
        public IEnumerable<Selectable> AllSubjects { get; set; } = new List<Selectable>();
        public IEnumerable<Selectable> AllTuitionTypes { get; set; } = new List<Selectable>();

        public TuitionPartnerSearchResultsPage? Results { get; set; }
    }

    public class Handler : IRequestHandler<Query, Command>
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

        public async Task<Command> Handle(Query request, CancellationToken cancellationToken)
        {
            if (request.Postcode == null) throw new FluentValidation.ValidationException("Enter a postcode");
            var loc = await locationService.GetLocationFilterParametersAsync(request.Postcode);
            var subjects = await db.Subjects.Where(s => request.Subjects.Contains(s.Name)).ToListAsync(cancellationToken);

            var allSubjects = await mediator.Send(new Subjects.Query { Subjects = request.Subjects }, cancellationToken);

            var cmd = new SearchTuitionPartnerHandler.Command
            {
                OrderBy = TuitionPartnerOrderBy.Name,
                LocalAuthorityDistrictCode = loc.LocalAuthorityDistrictCode,
                SubjectIds = subjects.Select(x => x.Id),
            };

            return new()
            {
                AllSubjects = allSubjects.AllSubjects,
                Results = await mediator.Send(cmd, cancellationToken),
            };
        }
    }
}