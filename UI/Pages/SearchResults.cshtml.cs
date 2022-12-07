using Application;
using Application.Commands;
using Application.Constants;
using Application.Extensions;
using Application.Queries;
using Domain;
using Domain.Enums;
using Domain.Search;
using Domain.Search.ShortlistTuitionPartners;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UI.Enums;
using UI.Extensions;
using UI.Models;
using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace UI.Pages;

public class SearchResults : PageModel
{
    private const string DefaultLocalAuthorityValue = "Not Provided";
    private readonly IMediator _mediator;
    public SearchResults(IMediator mediator) => _mediator = mediator;

    public ResultsModel Data { get; set; } = new();
    [BindProperty(SupportsGet = true)] public List<string> ShortlistedTuitionPartners { get; set; } = new();
    public List<SelectableTuitionPartnerModel> SelectableTuitionPartners { get; set; } = new();
    public int TotalShortlistedTuitionPartners { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? LocalAuthorityNameHolder { get; set; } = DefaultLocalAuthorityValue;


    public async Task OnGet(Query data)
    {
        data.TuitionType ??= Enums.TuitionType.Any;
        Data = await _mediator.Send(data);
        Data.From = ReferrerList.SearchResults;

        if (!Data.Validation.IsValid)
            foreach (var error in Data.Validation.Errors)
                ModelState.AddModelError($"Data.{error.PropertyName}", error.ErrorMessage);

        await SetUpShortlistTuitionPartnerFunctionality();
    }

    public async Task OnGetClearAllFilters(string postcode)
    {
        Data = await _mediator.Send(new Query
        { Postcode = postcode, Subjects = null, TuitionType = Enums.TuitionType.Any, KeyStages = null });

        await SetUpShortlistTuitionPartnerFunctionality();
    }

    public async Task<JsonResult> OnGetAddShortlistedTuitionPartner(string tuitionPartnerSeoUrl,
        string localAuthority)
    {
        var response = new UpdateTuitionPartnerResult(false, TotalShortlistedTuitionPartners);

        if (!IsParamValid(tuitionPartnerSeoUrl) || !IsParamValid(localAuthority))
            return new JsonResult(response);

        localAuthority = localAuthority.Trim();
        TotalShortlistedTuitionPartners = (await GetShortlistedTuitionPartnersByLocalAuthority(localAuthority)).Count();

        tuitionPartnerSeoUrl = tuitionPartnerSeoUrl.Trim();
        var commandParameter = new ShortlistedTuitionPartner(tuitionPartnerSeoUrl, localAuthority);
        var result = await _mediator.Send(new AddTuitionPartnerToShortlistCommand(commandParameter));

        //if unable to update storage that keeps track of shortlisted tuition partners
        if (result <= 0) return new JsonResult(response);

        // if update is successful
        TotalShortlistedTuitionPartners++;
        return GetJsonResult(TotalShortlistedTuitionPartners);
    }


    public async Task<JsonResult> OnGetRemoveShortlistedTuitionPartner(string tuitionPartnerSeoUrl,
        string localAuthority)
    {
        var response = new UpdateTuitionPartnerResult(false, TotalShortlistedTuitionPartners);

        if (!IsParamValid(tuitionPartnerSeoUrl) && !IsParamValid(localAuthority)) return new JsonResult(response);

        localAuthority = localAuthority.Trim();
        TotalShortlistedTuitionPartners = (await GetShortlistedTuitionPartnersByLocalAuthority(localAuthority)).Count();

        tuitionPartnerSeoUrl = tuitionPartnerSeoUrl.Trim();
        var result = await _mediator.Send(new RemoveTuitionPartnerCommand(tuitionPartnerSeoUrl, localAuthority));

        //if unable to update storage that keeps track of shortlisted tuition partners
        if (result <= 0) return new JsonResult(response);

        // if update is successful
        TotalShortlistedTuitionPartners--;

        return GetJsonResult(TotalShortlistedTuitionPartners);
    }

    private async Task SetUpShortlistTuitionPartnerFunctionality()
    {
        if (Data.Results != null)
        {
            SelectableTuitionPartners = await GetCorrectlySetTuitionPartners(Data, ShortlistedTuitionPartners);
            TotalShortlistedTuitionPartners = SelectableTuitionPartners.Count(stp => stp.IsSelected);
        }
    }

    private async Task<List<SelectableTuitionPartnerModel>> GetCorrectlySetTuitionPartners(
        ResultsModel resultsModel, IEnumerable<string> selectedTuitionPartners)
    {
        var correctlySetTuitionPartners = new List<SelectableTuitionPartnerModel>();
        selectedTuitionPartners = selectedTuitionPartners.ToList();

        if (selectedTuitionPartners.Any() &&
            IsSameLocalAuthority(resultsModel.Results?.LocalAuthorityName ?? DefaultLocalAuthorityValue,
                LocalAuthorityNameHolder ?? DefaultLocalAuthorityValue))
        {
            await AddAllSelectedTuitionPartnersToStorage(selectedTuitionPartners);
            correctlySetTuitionPartners.AddRange(selectedTuitionPartners
                .Select(tpSeoUrl => new SelectableTuitionPartnerModel(tpSeoUrl, true)));
        }

        if (!selectedTuitionPartners.Any() && IsSameLocalAuthority(
                resultsModel.Results?.LocalAuthorityName ?? DefaultLocalAuthorityValue,
                LocalAuthorityNameHolder ?? DefaultLocalAuthorityValue))
        {
            await _mediator.Send(new RemoveTuitionPartnersByLocalAuthorityCommand(
                LocalAuthorityNameHolder ?? DefaultLocalAuthorityValue));
        }

        if (!IsSameLocalAuthority(resultsModel.Results?.LocalAuthorityName ?? DefaultLocalAuthorityValue,
                LocalAuthorityNameHolder ?? DefaultLocalAuthorityValue))
        {
            var result = (await GetShortlistedTuitionPartnersByLocalAuthority
                (resultsModel.Results?.LocalAuthorityName ?? DefaultLocalAuthorityValue));
            correctlySetTuitionPartners.AddRange(result
                .Where(tp => tp.LocalAuthorityName == resultsModel.Results?.LocalAuthorityName)
                .Select(tp => new SelectableTuitionPartnerModel(tp.SeoUrl, true)));
        }

        var tuitionPartners = resultsModel.Results?.Results.Select(tuitionPartner =>
            new SelectableTuitionPartnerModel(tuitionPartner.SeoUrl)).ToList();

        if (tuitionPartners == null) return correctlySetTuitionPartners;
        //Set up returned value to have the selected option set correctly
        tuitionPartners = tuitionPartners
            .Except(correctlySetTuitionPartners, new SelectableTuitionPartnerModelComparer()).ToList();
        //Add to list returned which is what the user eventually sees.
        correctlySetTuitionPartners.AddRange(tuitionPartners);

        return correctlySetTuitionPartners;
    }

    private async Task<IEnumerable<ShortlistedTuitionPartner>> GetShortlistedTuitionPartnersByLocalAuthority(
        string localAuthority)
        => (await _mediator.Send(new GetShortlistedTuitionPartnersByLocalAuthorityQuery(
            localAuthority)));

    private bool IsSameLocalAuthority(string currentLocalAuthority, string previousLocalAuthority) =>
        currentLocalAuthority == previousLocalAuthority;


    private async Task AddAllSelectedTuitionPartnersToStorage(IEnumerable<string> shortlistedTuitionPartners)
    {
        var commandParameters = ShortlistedTuitionPartners.Select(selectedTuitionPartner =>
                new ShortlistedTuitionPartner(selectedTuitionPartner,
                    Data.Results?.LocalAuthorityName ?? DefaultLocalAuthorityValue))
            .ToList();

        await _mediator.Send(new AddTuitionPartnersToShortlistCommand(commandParameters));
    }

    private bool IsParamValid(string value) => !(string.IsNullOrEmpty(value) || value.ToLower().Equals("undefined"));

    private JsonResult GetJsonResult(int totalShortlistedTuitionPartners) =>
        new(new UpdateTuitionPartnerResult(true, totalShortlistedTuitionPartners));

    public record Query : SearchModel, IRequest<ResultsModel>;

    public record ResultsModel : SearchModel
    {
        public ResultsModel()
        {
        }

        public ResultsModel(SearchModel query) : base(query)
        {
        }

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

        public Handler(ILocationFilterService locationService, ITuitionPartnerService tuitionPartnerService,
            INtpDbContext db, IMediator mediator)
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

        private async Task<Dictionary<Enums.KeyStage, Selectable<string>[]>> GetSubjectsList(Query request,
            CancellationToken cancellationToken)
        {
            return await _mediator.Send(new WhichSubjects.Query
            {
                KeyStages = AllKeyStages,
                Subjects = request.Subjects,
            }, cancellationToken);
        }

        private async Task<IResult<TuitionPartnersResult>> GetSearchResults(Query request,
            CancellationToken cancellationToken)
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

        private async Task<IResult<LocationFilterParameters>> GetSearchLocation(Query request,
            CancellationToken cancellationToken)
        {
            var validationResults = await new Validator().ValidateAsync(request, cancellationToken);

            if (string.IsNullOrWhiteSpace(request.Postcode))
                return Result.Success(new LocationFilterParameters());

            return !validationResults.IsValid
                ? Result.Invalid<LocationFilterParameters>(validationResults.Errors)
                : (await _locationService.GetLocationFilterParametersAsync(request.Postcode!)).TryValidate();
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

            var tuitionPartnersIds = await _tuitionPartnerService.GetTuitionPartnersFilteredAsync(
                new TuitionPartnersFilter
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
                RandomSeed = TuitionPartnerOrdering.RandomSeedGeneration(parameters.LocalAuthorityDistrictCode,
                    parameters.Postcode, subjectFilterIds, tuitionFilterId)
            });

            return tuitionPartners;
        }
    }
}