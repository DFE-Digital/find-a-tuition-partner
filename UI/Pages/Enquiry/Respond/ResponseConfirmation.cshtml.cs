using Application.Common.Models;

namespace UI.Pages.Enquiry.Respond
{
    public class ResponseConfirmation : PageModel
    {
        public SearchModel Data { get; set; } = new();
        public void OnGet()
        {
        }
    }
}
