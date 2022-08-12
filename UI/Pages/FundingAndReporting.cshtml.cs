using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Pages
{
    public class FundingAndReporting : PageModel
    {
        public string? returnPath { get; set; }


        public IActionResult OnGet()
        {
            returnPath = Request.Headers["Referer"].ToString();

            return Page();
        }

        public IActionResult OnPost(string returnUrl)
        {
            returnPath = Request.Headers["Referer"].ToString();

            return Page();
        }
    }
}
