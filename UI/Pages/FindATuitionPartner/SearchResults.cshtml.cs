using Application;
using Application.Extensions;
using Application.Handlers;
using Domain.Search;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace UI.Pages.FindATuitionPartner;

public class SearchResults : PageModel
{
    private readonly IMediator mediator;

    public SearchResults(IMediator mediator) => this.mediator = mediator;

    public Command Data { get; set; } = new();

    public async Task OnGet(Query query)
    {
        Data = await mediator.Send(query);
        if (!Data.Validation.IsValid)
            foreach (var error in Data.Validation.Errors)
                ModelState.AddModelError($"Data.{error.PropertyName}", error.ErrorMessage);
    }

    public record Query : SearchModel, IRequest<Command> { }

    public record Command : SearchModel
    {
        public Command() { }
        public Command(SearchModel query) : base(query) { }
        public string? LocalAuthority { get; set; }
        public Dictionary<KeyStage, Selectable<string>[]> AllSubjects { get; set; } = new();
        public IEnumerable<TuitionType> AllTuitionTypes { get; set; } = new List<TuitionType>();

        public TuitionPartnerSearchResultsPage? Results { get; set; }
        public ValidationResult Validation { get; internal set; } = new ValidationResult();
    }

    private class Validator : AbstractValidator<Query>
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

            RuleForEach(m => m.Subjects)
                .Must(x => KeyStageSubject.TryParse(x, out var _));
        }
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
            var validator = new Validator();
            var validationResults = await validator.ValidateAsync(request, cancellationToken);

            var allSubjects = await mediator.Send(new WhichSubjects.Query 
            {
                KeyStages = new[]
                {
                    KeyStage.KeyStage1,
                    KeyStage.KeyStage2,
                    KeyStage.KeyStage3,
                    KeyStage.KeyStage4,
                } ,
                Subjects = request.Subjects,
            }, cancellationToken);

            var localAuthority = "";
            TuitionPartnerSearchResultsPage? results = null;

            if (validationResults.IsValid)
            {
                var loc = await locationService.GetLocationFilterParametersAsync(request.Postcode!);
                localAuthority = loc?.LocalAuthority ?? "";

                var keyStageSubjects = request.Subjects?.ParseKeyStageSubjects() ?? Array.Empty<KeyStageSubject>();

                var subjects = await db.Subjects.Where(e => keyStageSubjects.Select(x => $"{x.KeyStage}-{x.Subject}".ToSeoUrl()).Contains(e.SeoUrl)).ToListAsync(cancellationToken);

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
                LocalAuthority = localAuthority,
                AllSubjects = allSubjects,
                Results = results,
                Validation = validationResults,
                AllTuitionTypes = new List<TuitionType> { TuitionType.Any, TuitionType.InSchool, TuitionType.Online },
            };
        }
    }
}