using Application.Common.Models.Enquiry.Respond;

namespace UI.Pages.Enquiry.Respond
{
    public class EnquirerViewTuitionPartnerDetails : PageModel
    {
        private readonly IMediator _mediator;
        [BindProperty] public EnquirerViewTuitionPartnerDetailsModel Data { get; set; } = new();

        [FromRoute(Name = "support-reference-number")] public string SupportReferenceNumber { get; set; } = string.Empty;

        [FromRoute(Name = "tuition-partner-seo-url")] public string TuitionPartnerSeoUrl { get; set; } = string.Empty;

        public EnquirerViewTuitionPartnerDetails(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var queryToken = Request.Query["Token"].ToString();

            var isValidMagicLink =
                await _mediator.Send(new IsValidMagicLinkTokenQuery(queryToken, SupportReferenceNumber));

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
    }
}