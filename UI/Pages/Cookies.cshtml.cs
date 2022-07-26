
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Pages
{
    public class Cookies : PageModel
    {
		private const string ConsentCookieName = ".FindATuitionPartner.Consent";
		public IActionResult OnGet(bool? consent, string returnUrl)
		{
			if (consent.HasValue)
			{
				var cookieOptions = new CookieOptions { Expires = DateTime.Today.AddMonths(12), Secure = true };
				Response.Cookies.Append(ConsentCookieName, consent.Value.ToString(), cookieOptions);
				return new RedirectResult(returnUrl);
			}
			return Page();
		}
	}
}
