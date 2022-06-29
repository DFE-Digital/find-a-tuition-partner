using Application;
using Application.Handlers;
using Domain.Search;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace UI.Pages.FindATuitionPartner;

public class Results : PageModel
{
    private readonly IMediator mediator;

    public Results(IMediator mediator) => this.mediator = mediator;

    [BindProperty(SupportsGet = true)]
    public Command Data { get; set; } = new();

    public async Task OnGet()
    {
        Data = await mediator.Send(Data);
        if (!Data.Validation.IsValid)
            foreach (var error in Data.Validation.Errors)
                ModelState.AddModelError($"Data.{error.PropertyName}", error.ErrorMessage);
    }

    public record Command : SearchModel, IRequest<Command>
    {
        public Command() { }
        public Command(SearchModel query) : base(query) { }
        public IEnumerable<Selectable> AllSubjects { get; set; } = new List<Selectable>();
        public IEnumerable<TuitionType> AllTuitionTypes { get; set; } = new List<TuitionType>();

        public TuitionPartnerSearchResultsPage? Results { get; set; }
        public ValidationResult Validation { get; internal set; } = new ValidationResult();
    }

    private class Validator : AbstractValidator<Command>
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
            var validator = new Validator();
            var validationResults = await validator.ValidateAsync(request, cancellationToken);

            var allSubjects = await mediator.Send(new Subjects.Query { Subjects = request.Subjects }, cancellationToken);

            TuitionPartnerSearchResultsPage? results = null;

            if (validationResults.IsValid)
            {
                var loc = await locationService.GetLocationFilterParametersAsync(request.Postcode!);

                var subjects = await db.Subjects.Where(s => request.Subjects.Contains(s.Name)).ToListAsync(cancellationToken);

                var cmd = new SearchTuitionPartnerHandler.Command
                {
                    OrderBy = TuitionPartnerOrderBy.Name,
                    LocalAuthorityDistrictCode = loc?.LocalAuthorityDistrictCode,
                    SubjectIds = subjects.Select(x => x.Id),
                    TuitionTypeId = request.TuitionType > 0 ? (int?)request.TuitionType : null,
                };

                results = await mediator.Send(cmd, cancellationToken);
            }

            return new(request)
            {
                AllSubjects = allSubjects.AllSubjects,
                Results = results,
                Validation = validationResults,
                AllTuitionTypes = new List<TuitionType> { TuitionType.Any, TuitionType.InPerson, TuitionType.Online },
            };
        }
    }
}