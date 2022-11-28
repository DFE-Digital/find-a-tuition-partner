using Application;
using Application.Constants;
using Application.Extensions;
using Domain;
using Domain.Enums;
using Domain.Search;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UI.Extensions;
using UI.Models;
using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace UI.Pages;

public class SearchResults : PageModel
{
    private readonly IMediator _mediator;

    public SearchResults(IMediator mediator) => _mediator = mediator;

    public ResultsModel Data { get; set; } = new();

    public async Task OnGet(Query data)
    {
        data.TuitionType ??= Enums.TuitionType.Any;
        Data = await _mediator.Send(data);

        if (!Data.Validation.IsValid)
            foreach (var error in Data.Validation.Errors)
                ModelState.AddModelError($"Data.{error.PropertyName}", error.ErrorMessage);
    }

    public async Task OnGetClearAllFilters(string postcode)
    {
        Data = await _mediator.Send(new Query { Postcode = postcode, Subjects = null, TuitionType = Enums.TuitionType.Any, KeyStages = null });
    }

    public record Query : SearchModel, IRequest<ResultsModel>;

    public record ResultsModel : SearchModel
    {
        public ResultsModel() { }
        public ResultsModel(SearchModel query) : base(query) { }
        public Dictionary<Enums.KeyStage, Selectable<string>[]> AllSubjects { get; set; } = new();
        public IEnumerable<Enums.TuitionType> AllTuitionTypes { get; set; } = new List<Enums.TuitionType>();

        public TuitionPartnersResult? Results { get; set; }
        public FluentValidationResult Validation { get; internal set; } = new();
    }

    private class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(m => m.Postcode)
                .Matches(StringConstants.PostcodeRegExp)
                .WithMessage("Enter a valid postcode")
                .When(m => !string.IsNullOrEmpty(m.Postcode));

            RuleForEach(m => m.Subjects)
                .Must(x => KeyStageSubject.TryParse(x, out var _));
        }
    }

    public class Handler : IRequestHandler<Query, ResultsModel>
    {
        private readonly ILocationFilterService _locationService;
        private readonly ITuitionPartnerService _tuitionPartnerService;
        private readonly IMediator _mediator;
        private readonly INtpDbContext _db;

        public Handler(ILocationFilterService locationService, ITuitionPartnerService tuitionPartnerService, INtpDbContext db, IMediator mediator)
        {
            _locationService = locationService;
            _tuitionPartnerService = tuitionPartnerService;
            _db = db;
            _mediator = mediator;
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
                    Results = searchResults.Data,
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

        private static Enums.KeyStage[] AllKeyStages =>
            new[]
            {
                Enums.KeyStage.KeyStage1,
                Enums.KeyStage.KeyStage2,
                Enums.KeyStage.KeyStage3,
                Enums.KeyStage.KeyStage4,
            };

        private static List<Enums.TuitionType> AllTuitionTypes =>
            new()
            {
                Enums.TuitionType.Any,
                Enums.TuitionType.InSchool,
                Enums.TuitionType.Online,
            };

        private async Task<Dictionary<Enums.KeyStage, Selectable<string>[]>> GetSubjectsList(Query request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new WhichSubjects.Query
            {
                KeyStages = AllKeyStages,
                Subjects = request.Subjects,
            }, cancellationToken);
        }

        private async Task<IResult<TuitionPartnersResult>> GetSearchResults(Query request, CancellationToken cancellationToken)
        {
            var location = await GetSearchLocation(request, cancellationToken);

            if (location is IErrorResult error)
            {
                return error.Cast<TuitionPartnersResult>();
            }

            var results = await FindTuitionPartners(
                        location.Data,
                        request,
                        cancellationToken);

            var result = new TuitionPartnersResult(results, location.Data.LocalAuthority);

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

        private async Task<IEnumerable<TuitionPartnerResult>> FindTuitionPartners(
            LocationFilterParameters parameters,
            Query request,
            CancellationToken cancellationToken)
        {
            var keyStageSubjects = request.Subjects?.ParseKeyStageSubjects() ?? Array.Empty<KeyStageSubject>();

            var subjects = await _db.Subjects.Where(e =>
                keyStageSubjects.Select(x => $"{x.KeyStage}-{x.Subject}".ToSeoUrl()).Contains(e.SeoUrl))
                .ToListAsync(cancellationToken);

            var subjectFilterIds = subjects.Select(x => x.Id);
            var tuitionFilterId = request.TuitionType > 0 ? (int?)request.TuitionType : null;

            var tuitionPartnersIds = await _tuitionPartnerService.GetTuitionPartnersFilteredAsync(new TuitionPartnersFilter
            {
                LocalAuthorityDistrictId = parameters.LocalAuthorityDistrictId,
                SubjectIds = subjectFilterIds,
                TuitionTypeId = tuitionFilterId
            }, cancellationToken);

            var tuitionPartners = await _tuitionPartnerService.GetTuitionPartnersAsync(new TuitionPartnerRequest
            {
                TuitionPartnerIds = tuitionPartnersIds,
                LocalAuthorityDistrictId = parameters.LocalAuthorityDistrictId,
                Urn = parameters.Urn
            }, cancellationToken);

            tuitionPartners = _tuitionPartnerService.OrderTuitionPartners(tuitionPartners, new TuitionPartnerOrdering
            {
                OrderBy = TuitionPartnerOrderBy.Random,
                Direction = OrderByDirection.Ascending,
                RandomSeed = TuitionPartnerOrdering.RandomSeedGeneration(parameters.LocalAuthorityDistrictCode, parameters.Postcode, subjectFilterIds, tuitionFilterId)
            });

            return tuitionPartners;
        }
    }
}