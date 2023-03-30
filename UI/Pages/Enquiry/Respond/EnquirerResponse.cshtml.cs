using System.Net;
using Application.Common.Models.Enquiry.Respond;

namespace UI.Pages.Enquiry.Respond;

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

        if (string.IsNullOrEmpty(SupportReferenceNumber) || string.IsNullOrEmpty(queryToken) || string.IsNullOrEmpty(TuitionPartnerSeoUrl))
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(SupportReferenceNumber))
        {
            Data.SupportReferenceNumber = SupportReferenceNumber;
        }
        if (!string.IsNullOrEmpty(TuitionPartnerSeoUrl))
        {
            Data.TuitionPartnerSeoUrl = TuitionPartnerSeoUrl;
        }

        var isValidMagicLink =
            await _mediator.Send(new IsValidMagicLinkTokenQuery(queryToken, SupportReferenceNumber));

        if (!isValidMagicLink)
        {
            return NotFound();
        }

        try
        {
            var getEnquirerViewResponseQuery = new GetEnquirerViewResponseQuery(SupportReferenceNumber, queryToken);

            var data = await _mediator.Send(getEnquirerViewResponseQuery);

            if (data != null)
            {
                Data = data with { EnquirerViewResponseToken = queryToken };
                HttpContext.AddLadNameToAnalytics<EnquirerResponse>(Data.LocalAuthorityDistrict);
                HttpContext.AddTuitionPartnerNameToAnalytics<EnquirerResponse>(Data.TuitionPartnerName);
                HttpContext.AddEnquirySupportReferenceNumberToAnalytics<EnquirerResponse>(Data.SupportReferenceNumber);
            }
        }
        catch
        {
            return Page();
        }

        return Page();
    }
}