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
    public ShortlistCheckboxModel ShortlistCheckboxModel = new();
    [BindProperty] public string? ShortlistedCheckbox { get; set; }

    public async Task<IActionResult> OnGetAsync(GetTuitionPartnerQuery query)
    {
        SearchModel = new SearchModel(query);

        if (string.IsNullOrWhiteSpace(query.Id))
            return ReturnNotFound($"Null or whitespace id '{query.Id}' provided");

        Data = await _mediator.Send(query);

        if (Data == null)
        {
            var seoUrl = query.Id.ToSeoUrl();

            if (query.Id == seoUrl) return ReturnNotFound($"No Tuition Partner found for id '{query.Id}'");

            _logger.LogInformation("Non SEO id '{Id}' provided. Redirecting to {SeoUrl}", query.Id, seoUrl);
            return RedirectToPage((query with { Id = seoUrl }).ToRouteData());
        }

        await GetShortlistCheckboxModel(Data.Name, Data.Id.Trim(), nameof(ShortlistedCheckbox));

        _logger.LogInformation("Tuition Partner {Name} found for id '{Id}'", Data.Name, query.Id);
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateShortlist(string seoUrl, string searchModel)
    {
        if (string.IsNullOrWhiteSpace(searchModel)) throw new ArgumentNullException(nameof(searchModel));

        SearchModel = JsonSerializer.Deserialize<SearchModel>(searchModel);

        if (string.IsNullOrWhiteSpace(ShortlistedCheckbox))
            await _mediator.Send(new RemoveTuitionPartnerCommand(seoUrl));

        if (!string.IsNullOrWhiteSpace(ShortlistedCheckbox))
            await _mediator.Send(new AddTuitionPartnerToShortlistCommand(seoUrl));

        return RedirectToPage("TuitionPartner", SearchModel?.ToRouteData());
    }

    public async Task<IActionResult> OnPostAddToShortlist([FromBody] string seoUrl)
    {
        if (!IsSeoUrlValid(seoUrl)) return GetShortlistJsonResult(false);

        await _mediator.Send(new AddTuitionPartnerToShortlistCommand(seoUrl.Trim()));

        return GetShortlistJsonResult(true);
    }

    public async Task<IActionResult> OnPostRemoveFromShortlist([FromBody] string seoUrl)
    {
        if (!IsSeoUrlValid(seoUrl)) return GetShortlistJsonResult(false);

        await _mediator.Send(new RemoveTuitionPartnerCommand(seoUrl.Trim()));

        return GetShortlistJsonResult(true);
    }

    private bool IsSeoUrlValid(string seoUrl) => !string.IsNullOrWhiteSpace(seoUrl);
    private JsonResult GetShortlistJsonResult(bool status) => new(new { Updated = status });

    private IActionResult ReturnNotFound(string logMessage)
    {
        _logger.LogWarning("{LogMessage}", logMessage);
        return NotFound();
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