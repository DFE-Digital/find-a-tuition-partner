using Application.Common.Models.Enquiry.Manage;
using Application.Queries.Enquiry;
using Application.Queries.Enquiry.Manage;

namespace UI.Pages.Enquiry.Manage;

public class EnquirerResponse : PageModel
{
    private readonly IMediator _mediator;
    [BindProperty] public EnquirerViewResponseModel Data { get; set; } = new();

    [FromRoute(Name = "support-reference-number")] public string SupportReferenceNumber { get; set; } = string.Empty;

    [FromRoute(Name = "tuition-partner-seo-url")] public string TuitionPartnerSeoUrl { get; set; } = string.Empty;

    public EnquirerResponse(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> OnGet()
    {
        var queryToken = Request.Query["Token"].ToString();

        var isValidMagicLink =
            await _mediator.Send(new IsValidMagicLinkTokenQuery(queryToken, SupportReferenceNumber));

        if (!isValidMagicLink)
        {
            return NotFound();
        }

        var enquirerViewResponseQuery = new GetEnquirerViewResponseQuery(SupportReferenceNumber, TuitionPartnerSeoUrl);

        var data = await _mediator.Send(enquirerViewResponseQuery);

        if (data == null)
        {
            return NotFound();
        }

        Data = data;
        Data.TuitionPartnerSeoUrl = TuitionPartnerSeoUrl;
        Data.Token = queryToken;

        HttpContext.AddLadNameToAnalytics<EnquirerResponse>(Data.LocalAuthorityDistrict);

        return Page();
    }
}