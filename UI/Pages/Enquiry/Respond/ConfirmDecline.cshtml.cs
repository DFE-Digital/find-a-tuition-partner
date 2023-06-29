using Application.Commands.Enquiry.Respond;
using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;
using Application.Queries.Enquiry;
using UI.Models;

namespace UI.Pages.Enquiry.Respond
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ConfirmDecline : ResponsePageModel<ConfirmDecline>
    {
        [BindProperty] public ViewAndCaptureEnquiryResponseModel Data { get; set; } = new();

        [FromRoute(Name = "support-reference-number")] public string SupportReferenceNumber { get; set; } = string.Empty;

        [FromRoute(Name = "tuition-partner-seo-url")] public string TuitionPartnerSeoUrl { get; set; } = string.Empty;

        public ConfirmDecline(ISessionService sessionService, IMediator mediator) : base(sessionService, mediator)
        {
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var queryToken = Request.Query["Token"].ToString();

            var isValidMagicLink =
                await _mediator.Send(new IsValidMagicLinkTokenQuery(queryToken, SupportReferenceNumber, TuitionPartnerSeoUrl, true));

            if (!isValidMagicLink)
            {
                return NotFound();
            }

            Data.SupportReferenceNumber = SupportReferenceNumber;
            Data.TuitionPartnerSeoUrl = TuitionPartnerSeoUrl;
            Data.Token = queryToken;

            var enquiryData = await _mediator.Send(new GetEnquirerViewAllResponsesQuery(SupportReferenceNumber));

            Data.LocalAuthorityDistrict = enquiryData.LocalAuthorityDistrict!;

            HttpContext.AddLadNameToAnalytics<ViewResponse>(Data.LocalAuthorityDistrict);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var isValidMagicLink =
                await _mediator.Send(new IsValidMagicLinkTokenQuery(Data.Token, Data.SupportReferenceNumber, Data.TuitionPartnerSeoUrl, true));

            if (!isValidMagicLink)
            {
                return NotFound();
            }

            var command = new DeclineEnquiryResponseCommand(Data.SupportReferenceNumber, Data.TuitionPartnerSeoUrl!);

            await _mediator.Send(command);

            HttpContext.AddLadNameToAnalytics<CheckYourAnswers>(Data.LocalAuthorityDistrict);

            var redirectPageUrl = $"/enquiry-response/{Data.TuitionPartnerSeoUrl}/{Data.SupportReferenceNumber}/declined-confirmation?Token={Data.Token}";

            return Redirect(redirectPageUrl);
        }
    }
}