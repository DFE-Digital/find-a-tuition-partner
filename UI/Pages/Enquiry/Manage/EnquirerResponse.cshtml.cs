using Application.Commands.Enquiry.Manage;
using Application.Common.Models.Enquiry.Manage;
using Application.Queries.Enquiry;
using Application.Queries.Enquiry.Manage;

namespace UI.Pages.Enquiry.Manage;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class EnquirerResponse : PageModel
{
    private readonly IMediator _mediator;
    [BindProperty] public EnquirerViewResponseModel Data { get; set; } = new();
    [BindProperty] public EnquirerResponseResultsModel EnquirerResponseResultsModel { get; set; } = new();

    [FromRoute(Name = "support-reference-number")] public string SupportReferenceNumber { get; set; } = string.Empty;

    [FromRoute(Name = "tuition-partner-seo-url")] public string TuitionPartnerSeoUrl { get; set; } = string.Empty;

    public EnquirerResponse(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> OnGet(EnquirerResponseResultsModel enquirerResponseResultsModel)
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

        if (data.EnquiryResponseStatus == EnquiryResponseStatus.Unread ||
            data.EnquiryResponseStatus == EnquiryResponseStatus.NotSet)
        {
            data.EnquiryResponseStatus = EnquiryResponseStatus.Undecided;
            await _mediator.Send(new UpdateEnquiryResponseStatusCommand(SupportReferenceNumber, TuitionPartnerSeoUrl, EnquiryResponseStatus.Undecided));
        }

        Data = data;
        Data.TuitionPartnerSeoUrl = TuitionPartnerSeoUrl;
        Data.Token = queryToken;

        HttpContext.AddLadNameToAnalytics<EnquirerResponse>(Data.LocalAuthorityDistrict);

        EnquirerResponseResultsModel = enquirerResponseResultsModel;

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

        await _mediator.Send(new UpdateEnquiryResponseStatusCommand(SupportReferenceNumber, TuitionPartnerSeoUrl, EnquiryResponseStatus.Interested));

        var redirectPageUrl = $"/enquiry/{Data.SupportReferenceNumber}/{Data.TuitionPartnerSeoUrl}/contact-details?Token={Data.Token}&{EnquirerResponseResultsModel.ToQueryString()}";
        return Redirect(redirectPageUrl);
    }

    public async Task<IActionResult> OnGetIsUndecided(EnquirerViewResponseModel data, EnquirerResponseResultsModel enquirerResponseResultsModel)
    {
        var isValidMagicLink =
            await _mediator.Send(new IsValidMagicLinkTokenQuery(data.Token, SupportReferenceNumber, TuitionPartnerSeoUrl));

        if (!isValidMagicLink)
        {
            return NotFound();
        }

        await _mediator.Send(new UpdateEnquiryResponseStatusCommand(SupportReferenceNumber, TuitionPartnerSeoUrl, EnquiryResponseStatus.Undecided));

        var redirectPageUrl = $"/enquiry/{SupportReferenceNumber}?Token={data.Token}&{enquirerResponseResultsModel.ToQueryString()}#all-responses-table";
        return Redirect(redirectPageUrl);
    }
}