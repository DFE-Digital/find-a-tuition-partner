using Application.Common.Models.Enquiry.Respond;

namespace UI.Pages.Enquiry.Respond
{
    public class ResponseConfirmation : PageModel
    {
        public SubmittedConfirmationModel Data { get; set; } = new();
        public void OnGet(SubmittedConfirmationModel data)
        {
            Data = data;
            HttpContext.AddLadNameToAnalytics<ResponseConfirmation>(Data.LocalAuthorityDistrictName);
        }
    }
}
