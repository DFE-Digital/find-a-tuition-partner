using Application.Commands.Enquiry.Build;
using Application.Commands.Enquiry.Manage;
using Application.Common.Models.Enquiry.Manage;
using Application.Queries.Enquiry;
using Application.Queries.Enquiry.Manage;

namespace UI.Pages.Enquiry.Manage
{
    public class ConfirmNotInterested : PageModel
    {
        private readonly IMediator _mediator;
        [BindProperty] public EnquirerViewTuitionPartnerDetailsModel Data { get; set; } = new();
        [BindProperty] public EnquirerResponseResultsModel EnquirerResponseResultsModel { get; set; } = new();

        [FromRoute(Name = "support-reference-number")] public string SupportReferenceNumber { get; set; } = string.Empty;

        [FromRoute(Name = "tuition-partner-seo-url")] public string TuitionPartnerSeoUrl { get; set; } = string.Empty;

        public ConfirmNotInterested(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGetAsync(EnquirerResponseResultsModel enquirerResponseResultsModel)
        {
            var queryToken = Request.Query["Token"].ToString();

            var isValidMagicLink =
                await _mediator.Send(new IsValidMagicLinkTokenQuery(queryToken, SupportReferenceNumber, TuitionPartnerSeoUrl));

            if (!isValidMagicLink)
            {
                return NotFound();
            }

            var enquirerViewTuitionPartnerDetailsQuery = new GetEnquirerViewTuitionPartnerDetailsQuery(SupportReferenceNumber, TuitionPartnerSeoUrl);

            var data = await _mediator.Send(enquirerViewTuitionPartnerDetailsQuery);

            if (data == null)
            {
                return NotFound();
            }

            Data = data;
            Data.TuitionPartnerSeoUrl = TuitionPartnerSeoUrl;
            Data.Token = queryToken;

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

            await _mediator.Send(new UpdateEnquiryResponseStatusCommand(SupportReferenceNumber, TuitionPartnerSeoUrl, EnquiryResponseStatus.NotInterested));

            await _mediator.Send(new SendNotInterestedNotificationCommand(
                                    SupportReferenceNumber,
                                    TuitionPartnerSeoUrl,
                                    Data.TuitionPartnerEmailAddress,
                                    Data.TuitionPartnerName,
                                    Data.LocalAuthorityDistrict,
                                    Request.GetBaseServiceUrl()));

            var redirectPageUrl = $"/enquiry/{SupportReferenceNumber}/{TuitionPartnerSeoUrl}/not-interested-feedback?Token={Data.Token}&{EnquirerResponseResultsModel.ToQueryString()}";
            return Redirect(redirectPageUrl);
        }
    }
}