using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UI.Extensions;

namespace UI.Pages;

public class Cookies : PageModel
{
    public const string ConsentCookieName = ".FindATuitionPartner.Consent";

    [BindProperty]
    [Required(ErrorMessage = "You must select an option")]
    public bool? Consent { get; set; }
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }
    public bool PreferencesSet { get; set; }

    public IActionResult OnGet(bool? consent, bool? preferencesSet)
    {
        Consent = consent;
        if (preferencesSet.HasValue)
        {
            PreferencesSet = preferencesSet.Value;
            ReturnUrl = TempData.Peek<string>("ReturnUrl");
        }
        else
        {
            ReturnUrl = Request.Headers["Referer"].ToString();
        }
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
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();
        ReturnUrl = ReturnUrl;

        if (!string.IsNullOrEmpty(ReturnUrl)) TempData.Set("ReturnUrl", ReturnUrl);
        ApplyCookieConsent(Consent);

        return RedirectToPage(new { preferencesSet = true });
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