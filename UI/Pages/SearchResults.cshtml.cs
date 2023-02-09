using Application.Common.Interfaces;
using Application.Common.Models;
using Domain;
using Domain.Search;
using FluentValidationResult = FluentValidation.Results.ValidationResult;
using KeyStage = Domain.Enums.KeyStage;
using TuitionType = Domain.Enums.TuitionType;

namespace UI.Pages;
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class SearchResults : PageModel
{
    private readonly IMediator _mediator;
    public SearchResults(IMediator mediator) => _mediator = mediator;

    public ResultsModel Data { get; set; } = new();
    [BindProperty(SupportsGet = true)] public List<string> CompareListedTuitionPartners { get; set; } = new();
    public List<SelectableTuitionPartnerModel> SelectableTuitionPartners { get; set; } = new();
    public int TotalCompareListedTuitionPartners { get; set; }
    [BindProperty(SupportsGet = true)] public string? UpdateMyCompareList { get; set; }

    // Name keys : Tps => TuitionPartners,Tp => TuitionPartner

    public async Task OnGet(Query data)
    {
        await CommonOnGetPostLogic(data);
    }

    public async Task OnPost(Query data)
    {
        await CommonOnGetPostLogic(data);
    }

    public async Task OnGetClearAllFilters(string postcode)
    {
        Data = await _mediator.Send(new Query
        { Postcode = postcode, Subjects = null, TuitionType = TuitionType.Any, KeyStages = null });

        await SetSelectableTuitionPartners();
    }

    private async Task CommonOnGetPostLogic(Query data)
    {
        data.TuitionType ??= TuitionType.Any;
        if (data.KeyStages == null && data.Subjects != null)
        {
            data.KeyStages = Enum.GetValues(typeof(KeyStage)).Cast<KeyStage>()
                .Where(x => string.Join(" ", data.Subjects).Contains(x.ToString())).ToArray();
        }

        Data = await _mediator.Send(data);
        Data.From = ReferrerList.SearchResults;

        //Clear compare list TuitionType if has been changed on compare list
        if (data.PreviousTuitionType != null && data.PreviousTuitionType != data.TuitionType)
        {
            Data.CompareListTuitionType = null;
        }
        Data.PreviousTuitionType = data.TuitionType;

        if (!Data.Validation.IsValid)
            foreach (var error in Data.Validation.Errors)
                ModelState.AddModelError($"Data.{error.PropertyName}", error.ErrorMessage);

        await SetSelectableTuitionPartners();
    }
    private async Task SetSelectableTuitionPartners()
    {
        if (Data.Results != null)
            SelectableTuitionPartners = await GetSelectableTuitionPartners(Data, CompareListedTuitionPartners);
    }

    private async Task<List<SelectableTuitionPartnerModel>> GetSelectableTuitionPartners(
        ResultsModel resultsModel, IEnumerable<string> selectedTuitionPartners)
    {
        var selectableTuitionPartnerModels = new List<SelectableTuitionPartnerModel>();
        selectedTuitionPartners = selectedTuitionPartners.ToList();

        var selectableTps = resultsModel.Results?.Results.Select(tp => tp.SeoUrl).ToList();
        var compareListedTps = (await _mediator.Send(new GetAllCompareListTuitionPartnersQuery())).ToList();
        var compareListedTpsInSelectableTps = new List<string>();
        if (compareListedTps.Any() && selectableTps != null && selectableTps.Any())
        {
            compareListedTpsInSelectableTps =
                GetSelectableTpsPresentInCompareListedTps(selectableTps, compareListedTps).ToList();

            selectableTuitionPartnerModels.AddRange(compareListedTpsInSelectableTps
                .Select(tpSeoUrl => new SelectableTuitionPartnerModel(tpSeoUrl, true)));
        }


        if (!string.IsNullOrEmpty(UpdateMyCompareList))
        {
            await UpdateTuitionPartners(selectedTuitionPartners, compareListedTpsInSelectableTps, compareListedTps);

            selectableTuitionPartnerModels.Clear();
            selectableTuitionPartnerModels.AddRange(selectedTuitionPartners
                .Select(tpSeoUrl => new SelectableTuitionPartnerModel(tpSeoUrl, true)));
        }

        TotalCompareListedTuitionPartners = compareListedTps.Count;

        return GetSelectableTuitionPartnerModels(selectableTps, selectableTuitionPartnerModels);
    }

    private IEnumerable<string> GetSelectableTpsPresentInCompareListedTps(List<string> selectableTps,
        List<string> compareListedTps) =>
        selectableTps.Where(tp => compareListedTps.Any(stp => stp == tp)).ToList();


    private async Task UpdateTuitionPartners(
        IEnumerable<string> selectedTuitionPartners, List<string> compareListedTpsInSelectableTps,
        List<string> compareListedTps)
    {
        if (compareListedTpsInSelectableTps.Any())
        {
            foreach (var tp in compareListedTpsInSelectableTps.Where(tp => compareListedTps.Any(stp => stp == tp)))
            {
                compareListedTps.RemoveAll(stp => stp == tp);
            }
        }

        compareListedTps.AddRange(selectedTuitionPartners);

        if (compareListedTps.Any())
            await _mediator.Send(new AddTuitionPartnersToCompareListCommand(compareListedTps));

        if (!compareListedTps.Any())
            await _mediator.Send(new RemoveAllCompareListedTuitionPartnersCommand());
    }

    private List<SelectableTuitionPartnerModel> GetSelectableTuitionPartnerModels(
        List<string>? selectableTps, List<SelectableTuitionPartnerModel> selectableTuitionPartnerModels)
    {
        var tuitionPartners = selectableTps?.Select(seoUrl =>
            new SelectableTuitionPartnerModel(seoUrl)).ToList();

        if (tuitionPartners == null) return selectableTuitionPartnerModels;
        //Set up returned value to have the selected option set correctly
        tuitionPartners = tuitionPartners
            .Except(selectableTuitionPartnerModels, new SelectableTuitionPartnerModelComparer()).ToList();
        //Add to list returned which is what the user eventually sees.
        selectableTuitionPartnerModels.AddRange(tuitionPartners);

        return selectableTuitionPartnerModels;
    }
    public record Query : SearchModel, IRequest<ResultsModel>
    {
        public TuitionType? PreviousTuitionType { get; set; } = null;
    };

    public record ResultsModel : SearchModel
    {
        public ResultsModel()
        {
        }

        public ResultsModel(SearchModel query) : base(query)
        {
        }

        public Dictionary<KeyStage, Selectable<string>[]> AllSubjects { get; set; } = new();
        public IEnumerable<TuitionType> AllTuitionTypes { get; set; } = new List<TuitionType>();

        public TuitionPartnersResult? Results { get; set; }
        public FluentValidationResult Validation { get; internal set; } = new();
        public TuitionType? PreviousTuitionType { get; set; }
    }

    private sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(m => m.Postcode)
                .NotEmpty()
                .WithMessage("Enter a postcode");

            RuleFor(m => m.Postcode)
                .Matches(StringConstants.PostcodeRegExp)
                .WithMessage("Enter a real postcode")
                .When(m => !string.IsNullOrEmpty(m.Postcode));

            RuleForEach(m => m.Subjects)
                .Must(x => KeyStageSubject.TryParse(x, out var _));
        }
    }

    public class Handler : IRequestHandler<Query, ResultsModel>
    {
        private readonly ILocationFilterService _locationService;
        private readonly ITuitionPartnerService _tuitionPartnerService;
        private readonly ILookupDataService _lookupDataService;
        private readonly IMediator _mediator;

        public Handler(ILocationFilterService locationService, ITuitionPartnerService tuitionPartnerService,
            ILookupDataService lookupDataService, IMediator mediator)
        {
            _locationService = locationService;
            _tuitionPartnerService = tuitionPartnerService;
            _lookupDataService = lookupDataService;
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

        private async Task<Dictionary<KeyStage, Selectable<string>[]>> GetSubjectsList(Query request,
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

            var subjects = await _lookupDataService.GetSubjectsAsync(cancellationToken);

            var subjectFilterIds = subjects
                                    .Where(e => keyStageSubjects.Select(x => $"{x.KeyStage}-{x.Subject}".ToSeoUrl()).Contains(e.SeoUrl))
                                    .Select(x => x.Id);

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