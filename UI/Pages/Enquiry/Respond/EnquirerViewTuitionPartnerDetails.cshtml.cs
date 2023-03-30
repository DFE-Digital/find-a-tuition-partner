using System.Net;
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
                var getEnquirerViewTuitionPartnerDetailsQuery = new
                    GetEnquirerViewTuitionPartnerDetailsQuery(SupportReferenceNumber, queryToken);

                var data = await _mediator.Send(getEnquirerViewTuitionPartnerDetailsQuery);

                if (data != null)
                {
                    Data = data with { EnquirerViewResponseToken = queryToken };
                    HttpContext.AddLadNameToAnalytics<EnquirerViewTuitionPartnerDetails>(Data.LocalAuthorityDistrict);
                    HttpContext.AddTuitionPartnerNameToAnalytics<EnquirerViewTuitionPartnerDetails>(Data.TuitionPartnerName);
                    HttpContext.AddEnquirySupportReferenceNumberToAnalytics<EnquirerViewTuitionPartnerDetails>(Data.SupportReferenceNumber);
                }
            }
            catch
            {
                return Page();
            }

            return Page();
        }
    }
}