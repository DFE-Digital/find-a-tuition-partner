using Application.Common.Models.Enquiry.Manage;
using Application.Queries.Enquiry;
using Application.Queries.Enquiry.Manage;

namespace UI.Pages.Enquiry.Manage
{
    public class ConfirmNotInterested : PageModel
    {
        private readonly IMediator _mediator;
        [BindProperty] public EnquirerViewTuitionPartnerDetailsModel Data { get; set; } = new();

        [FromRoute(Name = "support-reference-number")] public string SupportReferenceNumber { get; set; } = string.Empty;

        [FromRoute(Name = "tuition-partner-seo-url")] public string TuitionPartnerSeoUrl { get; set; } = string.Empty;

        public ConfirmNotInterested(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGetAsync()
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

            //TODO - Update status to rejected, queue email and move to not interested feedback page

            var redirectPageUrl = $"/enquiry/{Data.SupportReferenceNumber}?Token={Data.Token}";
            return Redirect(redirectPageUrl);
        }
    }
}