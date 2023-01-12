using Domain;
using Domain.Enums;
using Domain.Search;
using FluentValidationResult = FluentValidation.Results.ValidationResult;

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
        data.TuitionType ??= Enums.TuitionType.Any;
        data.OrganisationGroupingType ??= Enums.OrganisationGroupingType.Any;
        Data = await _mediator.Send(data);
        Data.From = ReferrerList.SearchResults;

        if (!Data.Validation.IsValid)
            foreach (var error in Data.Validation.Errors)
                ModelState.AddModelError($"Data.{error.PropertyName}", error.ErrorMessage);

        // await _mediator.Send(new RemoveAllTuitionPartnersCommand());
        await SetSelectableTuitionPartners();
    }

    public async Task OnGetClearAllFilters(string postcode)
    {
        Data = await _mediator.Send(new Query
        { Postcode = postcode, Subjects = null, TuitionType = Enums.TuitionType.Any, OrganisationGroupingType = Enums.OrganisationGroupingType.Any, KeyStages = null });

        await SetSelectableTuitionPartners();
    }

    public async Task<JsonResult> OnGetAddShortlistedTuitionPartner(string tuitionPartnerSeoUrl)
    {
        var response = new UpdateTuitionPartnerResult(false, TotalShortlistedTuitionPartners);

        if (!IsParamValid(tuitionPartnerSeoUrl)) return new JsonResult(response);

        TotalShortlistedTuitionPartners = (await _mediator.Send(new GetAllShortlistedTuitionPartnersQuery())).Count();
        await _mediator.Send(new AddTuitionPartnerToShortlistCommand(tuitionPartnerSeoUrl.Trim()));
        // if update is successful
        TotalShortlistedTuitionPartners++;
        return GetJsonResult(TotalShortlistedTuitionPartners);
    }


    public async Task<JsonResult> OnGetRemoveShortlistedTuitionPartner(string tuitionPartnerSeoUrl)
    {
        var response = new UpdateTuitionPartnerResult(false, TotalShortlistedTuitionPartners);

        if (!IsParamValid(tuitionPartnerSeoUrl)) return new JsonResult(response);

        TotalShortlistedTuitionPartners = (await _mediator.Send(new GetAllShortlistedTuitionPartnersQuery())).Count();
        tuitionPartnerSeoUrl = tuitionPartnerSeoUrl.Trim();
        await _mediator.Send(new RemoveTuitionPartnerCommand(tuitionPartnerSeoUrl));
        // if update is successful
        TotalShortlistedTuitionPartners--;

        return GetJsonResult(TotalShortlistedTuitionPartners);
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
            foreach (var tp in shortlistedTpsInSelectableTps)
            {
                if (shortlistedTps.Any(stp => stp == tp))
                    shortlistedTps.RemoveAll(stp => stp == tp);
            }
        }

        shortlistedTps.AddRange(selectedTuitionPartners);

        if (shortlistedTps.Any())
            await _mediator.Send(new AddTuitionPartnersToShortlistCommand(shortlistedTps));

        if (!shortlistedTps.Any())
            await _mediator.Send(new RemoveAllTuitionPartnersCommand());
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

    private bool IsParamValid(string value) => !(string.IsNullOrWhiteSpace(value) || value.ToLower().Equals("undefined"));

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
        public IEnumerable<Enums.OrganisationGroupingType> AllOrganisationGroupingTypes { get; set; } = new List<Enums.OrganisationGroupingType>();

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
                AllOrganisationGroupingTypes = AllOrganisationGroupingTypes,
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

        private static List<Enums.OrganisationGroupingType> AllOrganisationGroupingTypes =>
            new()
            {
                Enums.OrganisationGroupingType.Any,
                Enums.OrganisationGroupingType.Charity,
                Enums.OrganisationGroupingType.NonCharity,
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
            bool? isTypeOfCharityFilter = request.OrganisationGroupingType == OrganisationGroupingType.Any ?
                null :
                request.OrganisationGroupingType == OrganisationGroupingType.Charity;

            var tuitionPartnersIds = await _tuitionPartnerService.GetTuitionPartnersFilteredAsync(
                new TuitionPartnersFilter
                {
                    LocalAuthorityDistrictId = parameters.LocalAuthorityDistrictId,
                    SubjectIds = subjectFilterIds,
                    TuitionTypeId = tuitionFilterId,
                    IsTypeOfCharity = isTypeOfCharityFilter,
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