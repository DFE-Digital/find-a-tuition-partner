using Application.Common.Models.Enquiry.Respond;
using Application.Queries.Enquiry;

namespace UI.Pages.Enquiry.Respond
{
    public class DeclinedConfirmation : PageModel
    {
        protected readonly IMediator _mediator;

        public DeclinedConfirmation(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty] public ViewAndCaptureEnquiryResponseModel Data { get; set; } = new();

        [FromRoute(Name = "support-reference-number")] public string SupportReferenceNumber { get; set; } = string.Empty;

        [FromRoute(Name = "tuition-partner-seo-url")] public string TuitionPartnerSeoUrl { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var queryToken = Request.Query["Token"].ToString();

            Data.SupportReferenceNumber = SupportReferenceNumber;
            Data.TuitionPartnerSeoUrl = TuitionPartnerSeoUrl;
            Data.Token = queryToken;

            var enquiryData = await _mediator.Send(new GetEnquirerViewAllResponsesQuery(SupportReferenceNumber));

            Data.LocalAuthorityDistrict = enquiryData.LocalAuthorityDistrict!;

            HttpContext.AddLadNameToAnalytics<ViewResponse>(Data.LocalAuthorityDistrict);

            return Page();
        }
    }
}