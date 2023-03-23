using Application.Common.Models;
using UI.Extensions;
using UI.Models;

namespace UI.Pages;

public class TuitionPartner : PageModel
{
    private readonly ILogger<TuitionPartner> _logger;
    private readonly IMediator _mediator;

    public TuitionPartner(ILogger<TuitionPartner> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public TuitionPartnerModel? Data { get; set; }
    public SearchModel? SearchModel { get; set; }
    public CompareListCheckboxModel CompareListCheckboxModel { get; set; } = new();

    [BindProperty] public string? CompareListedCheckbox { get; set; }

    public async Task<IActionResult> OnGetAsync(GetTuitionPartnerQueryModel query)
    {
        SearchModel = new SearchModel(query);

        if (string.IsNullOrWhiteSpace(query.Id))
            return ReturnNotFoundWithLogging($"Null or whitespace id '{query.Id}' provided", LogLevel.Information);

        var getTuitionPartnerQuery = new GetTuitionPartnerQuery(query.Id, query.ShowLocationsCovered, query.ShowFullPricing, query.ShowFullInfo)
        {
            SearchModel = SearchModel
        };

        Data = await _mediator.Send(getTuitionPartnerQuery);

        if (Data == null)
        {
            var seoUrl = query.Id.ToSeoUrl();

            if (query.Id == seoUrl)
                return ReturnNotFoundWithLogging($"No Tuition Partner found for id '{query.Id}'", LogLevel.Information);

            _logger.LogInformation("Non SEO id '{Id}' provided. Redirecting to {SeoUrl}", query.Id, seoUrl);
            return RedirectToPage((query with { Id = seoUrl }).ToRouteData());
        }

        await GetCompareListCheckboxModel(Data.Name, Data.Id.Trim(), nameof(CompareListedCheckbox));

        HttpContext.AddLadNameToAnalytics<TuitionPartner>(Data.LocalAuthorityDistrictName);

        _logger.LogInformation("Tuition Partner {Name} found for id '{Id}'", Data.Name, query.Id);
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateCompareList(string seoUrl, string searchModel)
    {
        ValidatePostUpdateCompareListArguments(seoUrl, searchModel);

        SearchModel = JsonSerializer.Deserialize<SearchModel>(searchModel);

        await HandleCompareListUpdate(seoUrl);

        return RedirectToPage("TuitionPartner", SearchModel?.ToRouteData());
    }

    private void ValidatePostUpdateCompareListArguments(string seoUrl, string searchModel)
    {
        if (IsStringWhitespaceOrNull(seoUrl)) throw GetArgumentException(nameof(seoUrl));
        if (IsStringWhitespaceOrNull(searchModel)) throw GetArgumentException(nameof(searchModel));
    }

    private bool IsStringWhitespaceOrNull(string? parameter) => string.IsNullOrWhiteSpace(parameter);
    private ArgumentException GetArgumentException(string name) => new($"{name} is null or whitespace");

    private IActionResult ReturnNotFoundWithLogging(string logMessage, LogLevel logLevel)
    {
        _logger.Log(logLevel, "{LogMessage}", logMessage);
        return NotFound();
    }

    private async Task HandleCompareListUpdate(string seoUrl)
    {
        if (IsStringWhitespaceOrNull(CompareListedCheckbox))
            await _mediator.Send(new RemoveCompareListedTuitionPartnerCommand(seoUrl));

        if (!IsStringWhitespaceOrNull(CompareListedCheckbox))
            await _mediator.Send(new AddTuitionPartnersToCompareListCommand(new List<string>() { seoUrl }));
    }

    private async Task GetCompareListCheckboxModel(string name, string seoUrl, string checkboxName)
    {
        CompareListCheckboxModel.Id = $"compare-list-tpInfo-cb-{seoUrl}";
        CompareListCheckboxModel.LabelValue = name;
        CompareListCheckboxModel.CheckboxName = checkboxName;
        CompareListCheckboxModel.IsCompareListed = await _mediator.Send(new IsTuitionPartnerCompareListQuery(seoUrl));
        CompareListCheckboxModel.SeoUrl = seoUrl;
    }
}