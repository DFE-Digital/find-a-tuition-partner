using Application.Commands.Enquiry.Build;
using Application.Commands.Enquiry.Manage;
using Application.Common.Models.Enquiry.Manage;
using Application.Queries.Enquiry;
using Application.Queries.Enquiry.Manage;

namespace UI.Pages.Enquiry.Manage
{
    public class NotInterestedFeedback : PageModel
    {
        private readonly IMediator _mediator;
        [BindProperty] public NotInterestedFeedbackModel Data { get; set; } = new();

        [FromRoute(Name = "support-reference-number")] public string SupportReferenceNumber { get; set; } = string.Empty;

        [FromRoute(Name = "tuition-partner-seo-url")] public string TuitionPartnerSeoUrl { get; set; } = string.Empty;

        public NotInterestedFeedback(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var queryToken = Request.Query["Token"].ToString();

            var isValidMagicLink =
                await _mediator.Send(new IsValidMagicLinkTokenQuery(queryToken, SupportReferenceNumber, TuitionPartnerSeoUrl)
                {
                    ValidateEnquiryResponseStatus = false
                });

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

            Data = new NotInterestedFeedbackModel()
            {
                SupportReferenceNumber = SupportReferenceNumber,
                TuitionPartnerSeoUrl = TuitionPartnerSeoUrl,
                Token = queryToken,
                TuitionPartnerName = data.TuitionPartnerName
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var isValidMagicLink =
                await _mediator.Send(new IsValidMagicLinkTokenQuery(Data.Token, SupportReferenceNumber, TuitionPartnerSeoUrl)
                {
                    ValidateEnquiryResponseStatus = false
                });

            if (!isValidMagicLink)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(TuitionPartnerSeoUrl))
            {
                await _mediator.Send(new UpdateNotInterestedReasonCommand(
                                        SupportReferenceNumber,
                                        TuitionPartnerSeoUrl,
                                        Data.NotInterestedFeedback!));
            }

            var redirectPageUrl = $"/enquiry/{Data.SupportReferenceNumber}?Token={Data.Token}";
            return Redirect(redirectPageUrl);
        }
    }
}