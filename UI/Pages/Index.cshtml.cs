using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Pages;

public partial class Index : PageModel
{
    public IActionResult OnGet()
    {
        return RedirectToPage("FindATuitionPartner/Index");
    }
}