using Application.Common.Interfaces;
using Application.Common.Models;
using Domain;
using Domain.Search;
using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace UI.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class CompareList : PageModel
{
    private readonly IMediator _mediator;

    public CompareList(IMediator mediator) => _mediator = mediator;

    public ResultsModel Data { get; set; } = new();

    public async Task<IActionResult> OnGet(Query data)
    {
        data.From = ReferrerList.CompareList;
        if (data.CompareListTuitionSetting == null && data.TuitionSetting != null)
        {
            data.CompareListTuitionSetting = data.TuitionSetting.Value;
        }

        data.KeyStages = data.KeyStages.UpdateFromSubjects(data.Subjects);

        var validator = new Validator();
        var results = await validator.ValidateAsync(data);
        if (!results.IsValid)
            return RedirectToPage(nameof(SearchResults), new SearchModel(data));

        Data = await _mediator.Send(data);

        HttpContext.AddLadNameToAnalytics<CompareList>(Data.Results);

        return Page();
    }

    public IActionResult OnPostApplyRefinement(Query data)
    {
        if (!ModelState.IsValid) return Page();

        return RedirectToPage(data);
    }

    public async Task<IActionResult> OnPostRemoveAsync(Query data, string? removeTuitionPartnerSeoUrl)
    {
        if (!ModelState.IsValid) return Page();

        if (!string.IsNullOrWhiteSpace(removeTuitionPartnerSeoUrl))
        {
            await _mediator.Send(new RemoveCompareListedTuitionPartnerCommand(removeTuitionPartnerSeoUrl));
        }

        return RedirectToPage(data);
    }

    public async Task<IActionResult> OnPostAddToCompareList([FromBody] AddToCompareListModel compareListModel)
    {
        var response = new CompareListedTuitionPartnerResult(false, compareListModel.TotalCompareListedTuitionPartners);

        if (IsStringWhitespaceOrNull(compareListModel.SeoUrl)) return GetJsonResult(response.IsCallSuccessful, response.TotalCompareListedTuitionPartners);

        response.IsCallSuccessful = await _mediator.Send(new AddTuitionPartnersToCompareListCommand(new List<string>() { compareListModel.SeoUrl.Trim() }));

        return GetJsonResult(response.IsCallSuccessful, compareListModel.TotalCompareListedTuitionPartners);
    }

    public async Task<IActionResult> OnPostRemoveFromCompareList([FromBody] RemoveFromCompareListModel compareListModel)
    {
        var response = new CompareListedTuitionPartnerResult(false, compareListModel.TotalCompareListedTuitionPartners);

        if (IsStringWhitespaceOrNull(compareListModel.SeoUrl)) return GetJsonResult(response.IsCallSuccessful, response.TotalCompareListedTuitionPartners);

        compareListModel.SeoUrl = compareListModel.SeoUrl.Trim();

        response.IsCallSuccessful = await _mediator.Send(new RemoveCompareListedTuitionPartnerCommand(compareListModel.SeoUrl));

        return GetJsonResult(response.IsCallSuccessful, compareListModel.TotalCompareListedTuitionPartners);
    }

    private static JsonResult GetJsonResult(bool isCallSuccessful, int totalCompareListedTuitionPartners) =>
        new(new CompareListedTuitionPartnerResult(isCallSuccessful, totalCompareListedTuitionPartners));

    private static bool IsStringWhitespaceOrNull(string? parameter) => string.IsNullOrWhiteSpace(parameter);

    public record Query : SearchModel, IRequest<ResultsModel>
    {
        public int[]? SubjectIds { get; set; }
    };

    public record ResultsModel : SearchModel
    {
        public ResultsModel() { }
        public ResultsModel(SearchModel query) : base(query) { }

        public TuitionPartnersResult? Results { get; set; }

        public IEnumerable<TuitionPartnerResult>? InvalidTPs { get; set; }

        public FluentValidationResult Validation { get; internal set; } = new();

        public string GetAriaSort(TuitionPartnerOrderBy matchedOrderBy)
        {
            string result;
            if (CompareListOrderBy != matchedOrderBy)
            {
                result = "none";
            }
            else
            {
                result = CompareListOrderByDirection == OrderByDirection.Ascending ? "ascending" : "descending";
            }
            return result;
        }

        public Dictionary<string, string> GetSortRouteData(TuitionPartnerOrderBy matchedOrderBy)
        {
            return (this with { CompareListOrderBy = matchedOrderBy, CompareListOrderByDirection = (CompareListOrderBy == matchedOrderBy && CompareListOrderByDirection == OrderByDirection.Ascending) ? OrderByDirection.Descending : OrderByDirection.Ascending }).ToRouteData();
        }

        public IEnumerable<GroupSize> AllGroupSizes { get; set; } = new List<GroupSize>();
        public IEnumerable<Domain.Enums.TuitionSetting> AllTuitionSettings { get; set; } = new List<Domain.Enums.TuitionSetting>();
        public IEnumerable<string> KeyStageSubjectsFilteredLabels { get; set; } = new List<string>();

    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(m => m.Postcode)
                .NotEmpty()
                .WithMessage("Enter a postcode");

            RuleFor(m => m.Postcode)
                .Must(m => !string.IsNullOrEmpty(m.ToSanitisedPostcode()))
                .WithMessage("Enter a real postcode");
        }
    }

    public class Handler : IRequestHandler<Query, ResultsModel>
    {
        private readonly ILocationFilterService _locationService;
        private readonly ITuitionPartnerService _tuitionPartnerService;
        private readonly ILookupDataService _lookupDataService;
        private readonly ITuitionPartnerCompareListStorageService _tuitionPartnerCompareListStorageService;
        private readonly ILogger<TuitionPartner> _logger;

        public Handler(ILocationFilterService locationService, ITuitionPartnerService tuitionPartnerService,
            ILookupDataService lookupDataService, ITuitionPartnerCompareListStorageService tuitionPartnerCompareListStorageService, ILogger<TuitionPartner> logger)
        {
            _locationService = locationService;
            _tuitionPartnerService = tuitionPartnerService;
            _lookupDataService = lookupDataService;
            _tuitionPartnerCompareListStorageService = tuitionPartnerCompareListStorageService;
            _logger = logger;
        }

        public async Task<ResultsModel> Handle(Query request, CancellationToken cancellationToken)
        {
            var queryResponse = new ResultsModel(request) with
            {
                AllTuitionSettings = EnumExtensions.GetAllEnums<Domain.Enums.TuitionSetting>(),
                AllGroupSizes = EnumExtensions.GetAllEnums<GroupSize>()
            };

            var seoUrls = GetCompareListSeoUrls();

            var compareListOrderBy = request.CompareListOrderBy ?? TuitionPartnerOrderBy.SeoList;
            var compareListOrderByDirection = request.CompareListOrderByDirection ?? OrderByDirection.Ascending;

            List<string> keyStageSubjectsFilteredLabels = new();

            var keyStageSubjects = request.Subjects?.ParseKeyStageSubjects() ?? Array.Empty<KeyStageSubject>();

            var allSubjects = await _lookupDataService.GetSubjectsAsync(cancellationToken);

            var subjectFilters = allSubjects
                        .Where(e => keyStageSubjects.Select(x => $"{x.KeyStage}-{x.Subject}".ToSeoUrl()).Contains(e.SeoUrl));

            if (subjectFilters != null)
            {
                request.SubjectIds = subjectFilters!.Select(x => x.Id).ToArray();

                if (request.KeyStages != null)
                {
                    foreach (var keyStage in request.KeyStages)
                    {
                        var keyStageSubjectsFilteredLabel = keyStage.DisplayName() + ": " + subjectFilters.Where(x => x.SeoUrl.Contains(keyStage.DisplayName().ToSeoUrl())).Select(x => x.Name).Distinct().OrderBy(x => x).DisplayList();
                        keyStageSubjectsFilteredLabels.Add(keyStageSubjectsFilteredLabel);
                    }
                }
            }

            var searchResults = await GetCompareListResults(seoUrls, request, compareListOrderBy, compareListOrderByDirection, cancellationToken);

            IEnumerable<TuitionPartnerResult>? invalidResults = null;
            if (searchResults.IsSuccess && searchResults.Data.Count != seoUrls.Length)
            {
                var invalidSeoUrls = seoUrls.Where(e => !searchResults.Data.Results.Select(x => x.SeoUrl).Contains(e)).ToList();
                if (invalidSeoUrls.Any())
                {
                    invalidResults = await FindInvalidTuitionPartners(invalidSeoUrls.ToArray(), compareListOrderBy, compareListOrderByDirection, cancellationToken);
                    _logger.LogInformation("{Count} invalid SeoUrls '{InvalidSeoUrls}' provided on price comparison page for postcode '{Postcode}'", invalidSeoUrls.Count, string.Join(", ", invalidSeoUrls), request.Postcode);
                }
            }

            return searchResults switch
            {
                SuccessResult => queryResponse with
                {
                    Results = searchResults.Data,
                    InvalidTPs = invalidResults,
                    KeyStageSubjectsFilteredLabels = keyStageSubjectsFilteredLabels
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

        private string[] GetCompareListSeoUrls()
        {
            var compareListedTPs = _tuitionPartnerCompareListStorageService.GetAllTuitionPartners();

            var tuitionPartnersIds = compareListedTPs.Select(x => x).Distinct().ToArray();

            return tuitionPartnersIds;
        }

        private async Task<IResult<TuitionPartnersResult>> GetCompareListResults(
            string[] tuitionPartnerSeoUrls,
            Query request,
            TuitionPartnerOrderBy orderBy,
            OrderByDirection orderByDirection,
            CancellationToken cancellationToken)
        {
            var location = await GetSearchLocation(request, cancellationToken);

            if (location is IErrorResult error)
            {
                //Shouldn't be invalid, unless query string edited - since postcode on this page comes from previous page with validation
                _logger.LogWarning("Invalid postcode '{Postcode}' provided on price comparison page", request.Postcode);
                return error.Cast<TuitionPartnersResult>();
            }
            else if (location.Data.LocalAuthorityDistrictId is null)
            {
                _logger.LogError("Unable to get LocalAuthorityDistrictId for supplied postcode '{Postcode}' on TP price comparison page", request.Postcode);
                return Result.Error<TuitionPartnersResult>("Unable to get LocalAuthorityDistrictId for supplied postcode");
            }

            var results = await FindTuitionPartners(
                        tuitionPartnerSeoUrls,
                        orderBy,
                        orderByDirection,
                        location.Data,
                        new TuitionPartnersDataFilter()
                        {
                            GroupSize = (request.CompareListGroupSize == null || request.CompareListGroupSize == GroupSize.Any) ? null : (int)request.CompareListGroupSize,
                            TuitionSettingId = (request.CompareListTuitionSetting == null || request.CompareListTuitionSetting == Domain.Enums.TuitionSetting.NoPreference) ? null : (int)request.CompareListTuitionSetting,
                            SubjectIds = request.SubjectIds,
                            ShowWithVAT = request.CompareListShowWithVAT
                        },
                        cancellationToken);

            var result = new TuitionPartnersResult(results, location.Data.LocalAuthorityDistrict);

            return Result.Success(result);
        }
        private async Task<IResult<LocationFilterParameters>> GetSearchLocation(Query request, CancellationToken cancellationToken)
        {
            var validationResults = await new Validator().ValidateAsync(request, cancellationToken);

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
            string[] tuitionPartnerSeoUrls,
            TuitionPartnerOrderBy orderBy,
            OrderByDirection orderByDirection,
            LocationFilterParameters parameters,
            TuitionPartnersDataFilter tuitionPartnersDataFilter,
            CancellationToken cancellationToken)
        {

            var tuitionPartnersIds = await _tuitionPartnerService.GetTuitionPartnersFilteredAsync(new TuitionPartnersFilter
            {
                LocalAuthorityDistrictId = parameters.LocalAuthorityDistrictId,
                SeoUrls = tuitionPartnerSeoUrls
            }, cancellationToken);

            var tuitionPartners = await _tuitionPartnerService.GetTuitionPartnersAsync(new TuitionPartnerRequest
            {
                TuitionPartnerIds = tuitionPartnersIds,
                LocalAuthorityDistrictId = parameters.LocalAuthorityDistrictId,
                Urn = parameters?.Urn
            }, cancellationToken);

            tuitionPartners = _tuitionPartnerService.FilterTuitionPartnersData(tuitionPartners, tuitionPartnersDataFilter);

            tuitionPartners = _tuitionPartnerService.OrderTuitionPartners(tuitionPartners, new TuitionPartnerOrdering()
            {
                OrderBy = orderBy,
                Direction = orderByDirection,
                SeoUrlOrderBy = tuitionPartnerSeoUrls
            });

            return tuitionPartners;
        }

        private async Task<IEnumerable<TuitionPartnerResult>> FindInvalidTuitionPartners(
            string[] tuitionPartnerSeoUrls,
            TuitionPartnerOrderBy orderBy,
            OrderByDirection orderByDirection,
            CancellationToken cancellationToken)
        {

            var tuitionPartnersIds = await _tuitionPartnerService.GetTuitionPartnersFilteredAsync(new TuitionPartnersFilter
            {
                SeoUrls = tuitionPartnerSeoUrls
            }, cancellationToken);

            var tuitionPartners = await _tuitionPartnerService.GetTuitionPartnersAsync(new TuitionPartnerRequest
            {
                TuitionPartnerIds = tuitionPartnersIds
            }, cancellationToken);

            tuitionPartners = _tuitionPartnerService.OrderTuitionPartners(tuitionPartners, new TuitionPartnerOrdering() { OrderBy = orderBy, Direction = orderByDirection, SeoUrlOrderBy = tuitionPartnerSeoUrls });

            return tuitionPartners;
        }
    }
}