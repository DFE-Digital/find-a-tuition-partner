using Application.Common.Models;

namespace UI.Pages.Enquiry.Build
{
    public class SubmittedConfirmation : PageModel
    {
        public SearchModel Data { get; set; } = new();

        public void OnGet(SearchModel data)
        {
            //TODO - Add enquirer magic link to view responses here
            //TODO - For cypress testing add in TP magic links, hidden on page - only add if not production
            Data = data;
        }
    }
}
