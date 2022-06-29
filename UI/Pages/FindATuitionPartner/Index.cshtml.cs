using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Pages.FindATuitionPartner
{
    public class Index : PageModel
    {
        public IActionResult OnGet() => RedirectToPage("Location");
    }
}