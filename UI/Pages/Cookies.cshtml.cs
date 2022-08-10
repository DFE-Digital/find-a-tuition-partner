using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace UI.Pages
{
    public class Cookies : PageModel
    {
        private const string ConsentCookieName = ".FindATuitionPartner.Consent";

        [BindProperty]
        [Required(ErrorMessage = "You must select an option")]
        public bool? Consent { get; set; }
        public bool PreferencesSet { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public IActionResult OnGet(bool? consent)
        {
            Consent = consent;

            if (Request.Cookies.ContainsKey(ConsentCookieName))
            {
                var value = Request.Cookies[ConsentCookieName];
                if (value != null)
                {
                    Consent = bool.Parse(value);
                }
            }

            if (consent.HasValue)
            {
                ApplyCookieConsent(consent);
                if (!string.IsNullOrEmpty(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            PreferencesSet = true;
            ApplyCookieConsent(Consent);

            return Page();
        }

        private void ApplyCookieConsent(bool? consent)
        {
            if (consent.HasValue)
            {
                var cookieOptions = new CookieOptions { Expires = DateTime.Today.AddMonths(12), Secure = true };
                Response.Cookies.Append(ConsentCookieName, consent.Value.ToString(), cookieOptions);

                if (!consent.Value)
                {
                    foreach (var cookie in Request.Cookies.Keys)
                    {
                        if (cookie.StartsWith("_ga") || cookie.Equals("_gid"))
                        {
                            Response.Cookies.Delete(cookie);
                        }
                    }
                }
            }
        }
    }
}
