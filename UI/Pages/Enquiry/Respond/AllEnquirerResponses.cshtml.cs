using System.Net;
using Application.Common.Models.Enquiry.Respond;


namespace UI.Pages.Enquiry.Respond
{
    public class AllEnquirerResponses : PageModel
    {
        private readonly IMediator _mediator;

        public AllEnquirerResponses(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty] public EnquirerViewAllResponsesModel Data { get; set; } = new();

        [FromRoute(Name = "support-reference-number")] public string SupportReferenceNumber { get; set; } = string.Empty;

        public async Task<IActionResult> OnGet()
        {
            var queryToken = Request.Query["Token"].ToString();

            if (string.IsNullOrEmpty(SupportReferenceNumber) || string.IsNullOrEmpty(queryToken))
            {
                TempData["Status"] = HttpStatusCode.NotFound;
                return RedirectToPage(nameof(ErrorModel));
            }

            Data.SupportReferenceNumber = SupportReferenceNumber;

            try
            {
                var baseServiceUrl = Request.GetBaseServiceUrl();

                var isValidMagicLink =
                    await _mediator.Send(new IsValidMagicLinkTokenQuery(queryToken, SupportReferenceNumber));

                if (!isValidMagicLink)
                {
                    TempData["Status"] = HttpStatusCode.NotFound;
                    return RedirectToPage(nameof(ErrorModel));
                }

                var data = await _mediator.Send(new GetEnquirerViewAllResponsesQuery(baseServiceUrl, SupportReferenceNumber));
                if (data == null)
                {
                    TempData["Status"] = HttpStatusCode.NotFound;
                    return RedirectToPage(nameof(ErrorModel));
                }

                Data = data;

                HttpContext.AddLadNameToAnalytics<AllEnquirerResponses>(Data.LocalAuthorityDistrict);
                HttpContext.AddEnquirySupportReferenceNumberToAnalytics<AllEnquirerResponses>(Data.SupportReferenceNumber);
            }
            catch
            {
                return Page();
            }

            return Page();
        }
    }
}