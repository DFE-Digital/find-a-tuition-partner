﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Pages
{
    public class Cookies : PageModel
    {
		private const string ConsentCookieName = ".FindATuitionPartner.Consent";
		public bool? Consent { get; set; }
		public bool PreferencesSet { get; set; } = false;
		public string? returnPath { get; set; }
		public SearchModel? AllSearchData { get; set; }

		public IActionResult OnGet(bool? consent, string returnUrl)
		{
			AllSearchData = TempData.Peek<SearchModel>("AllSearchData");
			returnPath = returnUrl;

			if (consent.HasValue)
			{
				var cookieOptions = new CookieOptions { Expires = DateTime.Today.AddMonths(12), Secure = true };
				Response.Cookies.Append(ConsentCookieName, consent.Value.ToString(), cookieOptions);
				return new RedirectResult(returnUrl);
			}

			if (consent.HasValue)
			{
				PreferencesSet = true;

				ApplyCookieConsent(consent);

				if (!string.IsNullOrEmpty(returnUrl))
				{
					return Redirect(returnUrl);
				}

				return RedirectToPage(returnUrl);
			}

			return Page();
		}

		public IActionResult OnPost(bool? consent, string returnUrl)
		{
			returnPath = returnUrl;

			if (Request.Cookies.ContainsKey(ConsentCookieName))
			{
				Consent = bool.Parse(Request.Cookies[ConsentCookieName]);
			}

			if (consent.HasValue)
			{
				Consent = consent;
				PreferencesSet = true;

				var cookieOptions = new CookieOptions { Expires = DateTime.Today.AddMonths(6), Secure = true };
				Response.Cookies.Append(ConsentCookieName, consent.Value.ToString(), cookieOptions);

				if (!consent.Value)
				{
					ApplyCookieConsent(consent);
				}
				return Page();
			}

			return Page();
		}

		private void ApplyCookieConsent(bool? consent)
		{
			if (consent.HasValue)
			{
				var cookieOptions = new CookieOptions { Expires = DateTime.Today.AddMonths(6), Secure = true };
				Response.Cookies.Append(ConsentCookieName, consent.Value.ToString(), cookieOptions);
			}

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
