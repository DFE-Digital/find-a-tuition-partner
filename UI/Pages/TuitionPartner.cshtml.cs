using Application.Common.Models;
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
    public ShortlistCheckboxModel ShortlistCheckboxModel { get; set; } = new();

    [BindProperty] public string? ShortlistedCheckbox { get; set; }

    public async Task<IActionResult> OnGetAsync(GetTuitionPartnerQueryModel query)
    {
        SearchModel = new SearchModel(query);

        if (string.IsNullOrWhiteSpace(query.Id))
            return ReturnNotFoundWithLogging($"Null or whitespace id '{query.Id}' provided", LogLevel.Information);

        var getTuitionPartnerQuery = new GetTuitionPartnerQuery(query.Id, query.ShowLocationsCovered, query.ShowFullPricing)
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

        await GetShortlistCheckboxModel(Data.Name, Data.Id.Trim(), nameof(ShortlistedCheckbox));

        _logger.LogInformation("Tuition Partner {Name} found for id '{Id}'", Data.Name, query.Id);
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateShortlist(string seoUrl, string searchModel)
    {
        ValidatePostUpdateShortListArguments(seoUrl, searchModel);

        SearchModel = JsonSerializer.Deserialize<SearchModel>(searchModel);

        await HandleShortlistUpdate(seoUrl);

        return RedirectToPage("TuitionPartner", SearchModel?.ToRouteData());
    }

    private void ValidatePostUpdateShortListArguments(string seoUrl, string searchModel)
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

    private async Task HandleShortlistUpdate(string seoUrl)
    {
        if (IsStringWhitespaceOrNull(ShortlistedCheckbox))
            await _mediator.Send(new RemoveShortlistedTuitionPartnerCommand(seoUrl));

        if (!IsStringWhitespaceOrNull(ShortlistedCheckbox))
            await _mediator.Send(new AddTuitionPartnersToShortlistCommand(new List<string>() { seoUrl }));
    }

    private async Task GetShortlistCheckboxModel(string name, string seoUrl, string checkboxName)
    {
        ShortlistCheckboxModel.Id = $"shortlist-tpInfo-cb-{seoUrl}";
        ShortlistCheckboxModel.LabelValue = name;
        ShortlistCheckboxModel.CheckboxName = checkboxName;
        ShortlistCheckboxModel.IsShortlisted = await _mediator.Send(new IsTuitionPartnerShortlistedQuery(seoUrl));
        ShortlistCheckboxModel.SeoUrl = seoUrl;
    }
}