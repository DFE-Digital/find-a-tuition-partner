using Application;
using Application.Extensions;
using Application.Handlers;
using Domain;
using Domain.Search;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UI.Extensions;
using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace UI.Pages;

public class SearchResults : PageModel
{
    private readonly IMediator mediator;

    public SearchResults(IMediator mediator) => this.mediator = mediator;

    public ResultsModel Data { get; set; } = new();

    public async Task OnGet(Query Data)
    {
        TempData.Set("AllSearchData", Data);

        Data.TuitionType ??= TuitionType.Any;
        this.Data = await mediator.Send(Data);

        TempData.Set("LocalAuthorityDistrictCode", this.Data.LocalAuthorityDistrictCode ?? "");

        if (!this.Data.Validation.IsValid)
            foreach (var error in this.Data.Validation.Errors)
                ModelState.AddModelError($"Data.{error.PropertyName}", error.ErrorMessage);
    }

    public record Query : SearchModel, IRequest<ResultsModel>;

    public record ResultsModel : SearchModel
    {
        public ResultsModel() { }
        public ResultsModel(SearchModel query) : base(query) { }
        public string? LocalAuthority { get; set; }
        public Dictionary<KeyStage, Selectable<string>[]> AllSubjects { get; set; } = new();
        public IEnumerable<TuitionType> AllTuitionTypes { get; set; } = new List<TuitionType>();

        public TuitionPartnerSearchResultsPage? Results { get; set; }
        public FluentValidationResult Validation { get; internal set; } = new();
        public string? LocalAuthorityDistrictCode { get; set; }

        public bool IsAnySubjectSelected
            => AllSubjects.SelectMany(x => x.Value).Any(x => x.Selected);

        public bool? ForceOpenAllSubjectFilters => IsAnySubjectSelected ? null : false;
    }

    private class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(m => m.Postcode)
                .Matches(@"[a-zA-Z]{1,2}([0-9]{1,2}|[0-9][a-zA-Z])\s*[0-9][a-zA-Z]{2}")
                .WithMessage("Enter a valid postcode")
                .When(m => !string.IsNullOrEmpty(m.Postcode));

            RuleForEach(m => m.Subjects)
                .Must(x => KeyStageSubject.TryParse(x, out var _));
        }
    }

    public class Handler : IRequestHandler<Query, ResultsModel>
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

        public async Task<ResultsModel> Handle(Query request, CancellationToken cancellationToken)
        {
            var queryResponse = new ResultsModel(request) with
            {
                AllSubjects = await GetSubjectsList(request, cancellationToken),
                AllTuitionTypes = AllTuitionTypes,
            };

            var searchResults = await GetSearchResults(request, cancellationToken);

            return searchResults switch
            {
                SuccessResult => queryResponse with
                {
                    LocalAuthority = searchResults.Data.LocalAuthorityDistrict?.LocalAuthority.Name,
                    Results = searchResults.Data,
                    LocalAuthorityDistrictCode = searchResults.Data.Request.LocalAuthorityDistrictCode,
                },
                Domain.ValidationResult error => queryResponse with
                {
                    Validation = new FluentValidationResult(error.Failures)
                },
                ErrorResult error => queryResponse with
                {
                    Validation = new FluentValidationResult(new[]
                    {
                        new ValidationFailure("Postcode", error.ToString()),
                    }),
                },
                _ => queryResponse with { Validation = UnknownError() },
            };

            static FluentValidationResult UnknownError() =>
                new(new[] { new ValidationFailure("", "An unknown problem occurred") });
        }

        private static KeyStage[] AllKeyStages =>
            new[]
            {
                KeyStage.KeyStage1,
                KeyStage.KeyStage2,
                KeyStage.KeyStage3,
                KeyStage.KeyStage4,
            };

        private static List<TuitionType> AllTuitionTypes =>
            new()
            {
                TuitionType.Any,
                TuitionType.InSchool,
                TuitionType.Online,
            };

        private async Task<Dictionary<KeyStage, Selectable<string>[]>> GetSubjectsList(Query request, CancellationToken cancellationToken)
        {
            return await mediator.Send(new WhichSubjects.Query
            {
                KeyStages = AllKeyStages,
                Subjects = request.Subjects,
            }, cancellationToken);
        }

        private async Task<IResult<TuitionPartnerSearchResultsPage>> GetSearchResults(Query request, CancellationToken cancellationToken)
        {
            var location = await GetSearchLocation(request, cancellationToken);

            if (location is IErrorResult error)
            {
                return error.Cast<TuitionPartnerSearchResultsPage>();
            }

            var results = await FindSubjectsMatchingFilter(
                        location.Data.LocalAuthorityDistrictCode,
                        request,
                        cancellationToken);

            return Result.Success(results);
        }

        private async Task<IResult<LocationFilterParameters>> GetSearchLocation(Query request, CancellationToken cancellationToken)
        {
            var validationResults = await new Validator().ValidateAsync(request, cancellationToken);

            if (string.IsNullOrEmpty(request.Postcode))
                return Result.Success(new LocationFilterParameters { });

            if (!validationResults.IsValid)
            {
                return Result.Invalid<LocationFilterParameters>(validationResults.Errors);
            }
            else
            {
                return (await locationService.GetLocationFilterParametersAsync(request.Postcode!)).TryValidate();
            }
        }

        private async Task<TuitionPartnerSearchResultsPage> FindSubjectsMatchingFilter(
            string? localAuthorityDisctict,
            Query request,
            CancellationToken cancellationToken)
        {
            var keyStageSubjects = request.Subjects?.ParseKeyStageSubjects() ?? Array.Empty<KeyStageSubject>();

            var subjects = await db.Subjects.Where(e =>
                keyStageSubjects.Select(x => $"{x.KeyStage}-{x.Subject}".ToSeoUrl()).Contains(e.SeoUrl))
                .ToListAsync(cancellationToken);

            return await mediator.Send(new SearchTuitionPartnerHandler.Command
            {
                OrderBy = TuitionPartnerOrderBy.Random,
                LocalAuthorityDistrictCode = localAuthorityDisctict,
                SubjectIds = subjects.Select(x => x.Id),
                TuitionTypeId = request.TuitionType > 0 ? (int?)request.TuitionType : null,
            }, cancellationToken);
        }
    }
}