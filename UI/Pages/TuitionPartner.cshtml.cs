using UI.MediatR.Queries;

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
    [BindProperty] public string? Shortlisted { get; set; }
    public ShortlistCheckboxModel ShortlistCheckboxModel = new();

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

        await GetShortlistCheckboxModel(Data.Name, Data.Id.Trim(), nameof(Shortlisted));

        _logger.LogInformation("Tuition Partner {Name} found for id '{Id}'", Data.Name, query.Id);
        return Page();
    }

    private IActionResult ReturnNotFound(string logMessage)
    {
        _logger.LogWarning(logMessage);
        return NotFound();
    }

    private async Task GetShortlistCheckboxModel(string name, string seoUrl, string checkboxName)
    {
        ShortlistCheckboxModel.Value = name;
        ShortlistCheckboxModel.CheckboxName = checkboxName;
        ShortlistCheckboxModel.IsShortlisted = await _mediator.Send(new IsTuitionPartnerShortlistedQuery(seoUrl));
        ShortlistCheckboxModel.SeoUrl = $"shortlist-tpInfo-cb-{name}";
    }
}