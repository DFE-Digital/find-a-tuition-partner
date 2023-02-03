using Application.Common.Interfaces;
using Application.Common.Models;
using Domain;
using Domain.Enums;
using Domain.Search;
using FluentValidationResult = FluentValidation.Results.ValidationResult;
using KeyStage = Domain.Enums.KeyStage;

namespace UI.Pages;
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class SearchResults : PageModel
{
    private readonly IMediator _mediator;
    public SearchResults(IMediator mediator) => _mediator = mediator;

    public ResultsModel Data { get; set; } = new();
    [BindProperty(SupportsGet = true)] public List<string> ShortlistedTuitionPartners { get; set; } = new();
    public List<SelectableTuitionPartnerModel> SelectableTuitionPartners { get; set; } = new();
    public int TotalShortlistedTuitionPartners { get; set; }
    [BindProperty(SupportsGet = true)] public string? UpdateMyShortlist { get; set; }

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
        { Postcode = postcode, Subjects = null, TuitionType = Domain.Enums.TuitionType.Any, KeyStages = null });

        await SetSelectableTuitionPartners();
    }

    private async Task CommonOnGetPostLogic(Query data)
    {
        data.TuitionType ??= Domain.Enums.TuitionType.Any;
        if (data.KeyStages == null && data.Subjects != null)
        {
            data.KeyStages = Enum.GetValues(typeof(KeyStage)).Cast<KeyStage>()
                .Where(x => string.Join(" ", data.Subjects).Contains(x.ToString())).ToArray();
        }

        Data = await _mediator.Send(data);
        Data.From = ReferrerList.SearchResults;

        //Clear shortlist TuitionType if has been changed on shortlist
        if (data.PreviousTuitionType != null && data.PreviousTuitionType != data.TuitionType)
        {
            Data.ShortlistTuitionType = null;
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
            SelectableTuitionPartners = await GetSelectableTuitionPartners(Data, ShortlistedTuitionPartners);
    }

    private async Task<List<SelectableTuitionPartnerModel>> GetSelectableTuitionPartners(
        ResultsModel resultsModel, IEnumerable<string> selectedTuitionPartners)
    {
        var selectableTuitionPartnerModels = new List<SelectableTuitionPartnerModel>();
        selectedTuitionPartners = selectedTuitionPartners.ToList();

        var selectableTps = resultsModel.Results?.Results.Select(tp => tp.SeoUrl).ToList();
        var shortlistedTps = (await _mediator.Send(new GetAllShortlistedTuitionPartnersQuery())).ToList();
        var shortlistedTpsInSelectableTps = new List<string>();
        if (shortlistedTps.Any() && selectableTps != null && selectableTps.Any())
        {
            shortlistedTpsInSelectableTps =
                GetSelectableTpsPresentInShortlistedTps(selectableTps, shortlistedTps).ToList();

            selectableTuitionPartnerModels.AddRange(shortlistedTpsInSelectableTps
                .Select(tpSeoUrl => new SelectableTuitionPartnerModel(tpSeoUrl, true)));
        }


        if (!string.IsNullOrEmpty(UpdateMyShortlist))
        {
            await UpdateTuitionPartners(selectedTuitionPartners, shortlistedTpsInSelectableTps, shortlistedTps);

            selectableTuitionPartnerModels.Clear();
            selectableTuitionPartnerModels.AddRange(selectedTuitionPartners
                .Select(tpSeoUrl => new SelectableTuitionPartnerModel(tpSeoUrl, true)));
        }

        TotalShortlistedTuitionPartners = shortlistedTps.Count;

        return GetSelectableTuitionPartnerModels(selectableTps, selectableTuitionPartnerModels);
    }

    private IEnumerable<string> GetSelectableTpsPresentInShortlistedTps(List<string> selectableTps,
        List<string> shortlistedTps) =>
        selectableTps.Where(tp => shortlistedTps.Any(stp => stp == tp)).ToList();


    private async Task UpdateTuitionPartners(
        IEnumerable<string> selectedTuitionPartners, List<string> shortlistedTpsInSelectableTps,
        List<string> shortlistedTps)
    {
        if (shortlistedTpsInSelectableTps.Any())
        {
            foreach (var tp in shortlistedTpsInSelectableTps.Where(tp => shortlistedTps.Any(stp => stp == tp)))
            {
                shortlistedTps.RemoveAll(stp => stp == tp);
            }
        }

        shortlistedTps.AddRange(selectedTuitionPartners);

        if (shortlistedTps.Any())
            await _mediator.Send(new AddTuitionPartnersToShortlistCommand(shortlistedTps));

        if (!shortlistedTps.Any())
            await _mediator.Send(new RemoveAllShortlistedTuitionPartnersCommand());
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
        public Domain.Enums.TuitionType? PreviousTuitionType { get; set; } = null;
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
        public IEnumerable<Domain.Enums.TuitionType> AllTuitionTypes { get; set; } = new List<Domain.Enums.TuitionType>();

        public TuitionPartnersResult? Results { get; set; }
        public FluentValidationResult Validation { get; internal set; } = new();
        public Domain.Enums.TuitionType? PreviousTuitionType { get; set; }
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

        private static List<Domain.Enums.TuitionType> AllTuitionTypes =>
            new()
            {
                Domain.Enums.TuitionType.Any,
                Domain.Enums.TuitionType.InSchool,
                Domain.Enums.TuitionType.Online,
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