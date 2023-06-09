using Application.Commands.Enquiry.Manage;
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
            await _mediator.Send(new IsValidMagicLinkTokenQuery(queryToken, SupportReferenceNumber, TuitionPartnerSeoUrl));

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

        //TODO - Test validation if return to rejected page and no caching on using back button

        if (data.EnquiryResponseStatus == EnquiryResponseStatus.Unread ||
            data.EnquiryResponseStatus == EnquiryResponseStatus.Unread)
        {
            data.EnquiryResponseStatus = EnquiryResponseStatus.Undecided;
            await _mediator.Send(new UpdateEnquiryStatusCommand(SupportReferenceNumber, TuitionPartnerSeoUrl, EnquiryResponseStatus.Undecided));
        }

        Data = data;
        Data.TuitionPartnerSeoUrl = TuitionPartnerSeoUrl;
        Data.Token = queryToken;

        HttpContext.AddLadNameToAnalytics<EnquirerResponse>(Data.LocalAuthorityDistrict);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var isValidMagicLink =
            await _mediator.Send(new IsValidMagicLinkTokenQuery(Data.Token, SupportReferenceNumber, TuitionPartnerSeoUrl));

        if (!isValidMagicLink)
        {
            return NotFound();
        }

        await _mediator.Send(new UpdateEnquiryStatusCommand(SupportReferenceNumber, TuitionPartnerSeoUrl, EnquiryResponseStatus.Interested));

        var redirectPageUrl = $"/enquiry/{Data.SupportReferenceNumber}/{Data.TuitionPartnerSeoUrl}/contact-details?Token={Data.Token}";
        return Redirect(redirectPageUrl);
    }

    public async Task<IActionResult> OnGetIsUndecided(EnquirerViewResponseModel data)
    {
        var isValidMagicLink =
            await _mediator.Send(new IsValidMagicLinkTokenQuery(data.Token, SupportReferenceNumber, TuitionPartnerSeoUrl));

        if (!isValidMagicLink)
        {
            return NotFound();
        }

        await _mediator.Send(new UpdateEnquiryStatusCommand(SupportReferenceNumber, TuitionPartnerSeoUrl, EnquiryResponseStatus.Undecided));

        var redirectPageUrl = $"/enquiry/{SupportReferenceNumber}?Token={data.Token}";
        return Redirect(redirectPageUrl);
    }
}