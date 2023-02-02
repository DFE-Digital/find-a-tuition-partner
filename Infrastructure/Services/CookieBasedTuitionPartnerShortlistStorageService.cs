using System.Text;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// The CookieBasedTuitionPartnerShortlistStorageService stores ShortlistedTuitionPartner details in a cookie
/// for 24 hours/one day from the time it was added.
/// </summary>
public class CookieBasedTuitionPartnerShortlistStorageService : ITuitionPartnerShortlistStorageService
{
    // Name keys : Tps => TuitionPartners,Tp => TuitionPartner
    private const string CookieName = ".FindATuitionPartner.Shortlist";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CookieBasedTuitionPartnerShortlistStorageService> _logger;

    public CookieBasedTuitionPartnerShortlistStorageService(
        IHttpContextAccessor httpContextAccessor, ILogger<CookieBasedTuitionPartnerShortlistStorageService> logger)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException($"{nameof(httpContextAccessor)}");
        _logger = logger ?? throw new ArgumentNullException($"{nameof(logger)}");
    }

    /// <summary>
    /// Adds a Shortlisted Tuition Partner to an implemented form of storage.
    /// </summary>
    /// <param name="shortlistedTuitionPartnersSeoUrl"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public bool AddTuitionPartners(IEnumerable<string> shortlistedTuitionPartnersSeoUrl)
    {
        shortlistedTuitionPartnersSeoUrl = shortlistedTuitionPartnersSeoUrl.ToList();
        var isFaultyData =
            shortlistedTuitionPartnersSeoUrl.Any(seoUrl => !IsShortlistedTuitionPartnerSeoUrlValid(seoUrl));

        if (isFaultyData)
            throw new InvalidOperationException(
                $"One or more of the values in {nameof(shortlistedTuitionPartnersSeoUrl)} is invalid");
        if (_httpContextAccessor.HttpContext == null) throw new ArgumentException($"{nameof(_httpContextAccessor.HttpContext)} is null");

        var isCallSuccessful = AddTuitionPartnersToCookie(shortlistedTuitionPartnersSeoUrl);

        return isCallSuccessful;
    }

    ///<inheritdoc/>
    public IEnumerable<string> GetAllTuitionPartners()
    {
        var cookie = _httpContextAccessor.HttpContext?.Request.Cookies[$"{CookieName}"];

        if (cookie == null) return new List<string>();

        var tuitionPartnersSeoUrlFromCookie = GetTuitionPartnersSeoUrlFromCookie(cookie);

        return tuitionPartnersSeoUrlFromCookie;
    }

    ///<inheritdoc />
    public bool RemoveTuitionPartner(string shortlistedTuitionPartnerSeoUrl)
    {
        var result = false;
        shortlistedTuitionPartnerSeoUrl = shortlistedTuitionPartnerSeoUrl.Trim();
        var valuesInCookie = GetAllTuitionPartners().ToList();

        if (!valuesInCookie.Contains(shortlistedTuitionPartnerSeoUrl, StringComparer.OrdinalIgnoreCase)) return result;

        valuesInCookie.RemoveAll(v => v.Equals(shortlistedTuitionPartnerSeoUrl, StringComparison.OrdinalIgnoreCase));

        result = RemoveAllTuitionPartners();

        if (valuesInCookie.Any() && !AddTuitionPartnersToCookie(valuesInCookie, true)) return result;

        return result;

    }

    ///<inheritdoc/>>
    public bool RemoveAllTuitionPartners()
    {
        try
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(CookieName);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while trying to delete the cookie {cookieName}. Error: {ex}", ex, CookieName);
            return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public bool IsTuitionPartnerShortlisted(string tuitionPartnerSeoUrl)
    {
        if (!IsShortlistedTuitionPartnerSeoUrlValid(tuitionPartnerSeoUrl))
            throw new InvalidOperationException($"{nameof(tuitionPartnerSeoUrl)} is invalid");

        var allShortlistedTps = GetAllTuitionPartners().ToList();
        return allShortlistedTps.Any(tp => tp == tuitionPartnerSeoUrl.Trim());
    }

    private bool IsShortlistedTuitionPartnerSeoUrlValid(string tuitionPartnerSeoUrl)
    {
        if (!string.IsNullOrEmpty(tuitionPartnerSeoUrl)) return true;

        _logger.LogError("An invalid {tuitionPartnerSeoUrl} provided", nameof(tuitionPartnerSeoUrl));

        return false;
    }

    private bool AddTuitionPartnersToCookie(IEnumerable<string> tuitionPartnersSeoUrls, bool isFromRemoveTuitionPartner = false)
    {
        var distinctTuitionPartnersSeoUrls = tuitionPartnersSeoUrls.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        var getExistingCookieValues = new List<string> { };

        if (!isFromRemoveTuitionPartner)
        {
            getExistingCookieValues = GetAllTuitionPartners().ToList();

            RemoveAllTuitionPartners();
        }

        if (distinctTuitionPartnersSeoUrls.Count > 1)
        {
            var unSelectedTpValuesToBeRemovedFromCookie = getExistingCookieValues.Except(distinctTuitionPartnersSeoUrls);

            getExistingCookieValues.RemoveAll(x => unSelectedTpValuesToBeRemovedFromCookie.Contains(x));
        }

        var selectedTpValuesToAddToCookie = distinctTuitionPartnersSeoUrls.Except(getExistingCookieValues);

        var cookieValuesToAdd = selectedTpValuesToAddToCookie as string[] ?? selectedTpValuesToAddToCookie.ToArray();
        var cookieValuesToAddToStringBuilder = new StringBuilder(cookieValuesToAdd.Count());

        if (getExistingCookieValues.Any())
        {
            cookieValuesToAddToStringBuilder.Append(string.Join("&", getExistingCookieValues));
        }

        if (cookieValuesToAdd.Any())
        {
            if (cookieValuesToAddToStringBuilder.Length > 0)
            {
                cookieValuesToAddToStringBuilder.Append("&");
            }
            cookieValuesToAddToStringBuilder.Append(string.Join("&", cookieValuesToAdd));
        }

        if (string.IsNullOrWhiteSpace(cookieValuesToAddToStringBuilder.ToString())) return false;

        try
        {
            _httpContextAccessor.HttpContext?.Response.Cookies
                .Append(CookieName, $"{cookieValuesToAddToStringBuilder}", GetCookieOptions());
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while trying to add the cookie {cookieName}. Error: {ex}", ex, CookieName);
            return false;
        }
        return true;
    }
    private static CookieOptions GetCookieOptions() => new() { Expires = DateTime.Now.AddDays(1), HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict };
    private IEnumerable<string> GetTuitionPartnersSeoUrlFromCookie(string cookie)
    {
        var split = cookie.Split("&", StringSplitOptions.RemoveEmptyEntries).ToList();

        return split.Select(Uri.UnescapeDataString).ToList();
    }
}