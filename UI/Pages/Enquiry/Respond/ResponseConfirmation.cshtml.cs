using Application.Common.Models.Enquiry.Respond;

namespace UI.Pages.Enquiry.Respond
{
    public class ResponseConfirmation : PageModel
    {
        public ResponseConfirmationModel Data { get; set; } = new();

        [FromRoute(Name = "support-reference-number")] public string SupportReferenceNumber { get; set; } = string.Empty;

        [FromRoute(Name = "tuition-partner-seo-url")] public string TuitionPartnerSeoUrl { get; set; } = string.Empty;

        public void OnGet(ResponseConfirmationModel data)
        {
            Data = data;
            Data.SupportReferenceNumber = SupportReferenceNumber;
        }
    }
}
