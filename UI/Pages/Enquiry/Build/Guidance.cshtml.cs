using Application.Common.Models;

namespace UI.Pages.Enquiry.Build
{
    public class Guidance : PageModel
    {
        public SearchModel Data { get; set; } = new();

        public void OnGet(SearchModel data) => Data = data;
    }
}
