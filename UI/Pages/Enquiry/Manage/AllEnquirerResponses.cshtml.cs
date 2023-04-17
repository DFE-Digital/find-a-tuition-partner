using Application.Common.Models.Enquiry.Manage;
using Application.Queries.Enquiry;

namespace UI.Pages.Enquiry.Manage
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

            var isValidMagicLink =
                await _mediator.Send(new IsValidMagicLinkTokenQuery(queryToken, SupportReferenceNumber));

            if (!isValidMagicLink)
            {
                return NotFound();
            }

            Data = await _mediator.Send(new GetEnquirerViewAllResponsesQuery(SupportReferenceNumber));

            Data.Token = queryToken;

            HttpContext.AddLadNameToAnalytics<AllEnquirerResponses>(Data.LocalAuthorityDistrict);

            return Page();
        }
    }
}