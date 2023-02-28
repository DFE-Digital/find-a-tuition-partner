using Application.Common.Models;

namespace UI.Pages.Enquiry.Build
{
    public class SubmittedConfirmation : PageModel
    {
        //TODO - Will enquirer magic link to view responses be shown here?
        //TODO - Confirm if there is a shorter enquiry ref shown
        public SearchModel Data { get; set; } = new();

        public void OnGet(SearchModel data) => Data = data;
    }
}
