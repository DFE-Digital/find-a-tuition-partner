using System.Text;
using Application.Common.Interfaces;

namespace UI.Services;

/// <summary>
/// The CookieBasedTuitionPartnerCompareListStorageService stores CompareListedTuitionPartner details in a cookie
/// for 24 hours/one day from the time it was added.
/// </summary>
public class CookieBasedTuitionPartnerCompareListStorageService : ITuitionPartnerCompareListStorageService
{
    // Name keys : Tps => TuitionPartners,Tp => TuitionPartner
    private const string CookieName = ".FindATuitionPartner.PriceComparisonList";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CookieBasedTuitionPartnerCompareListStorageService> _logger;

    public CookieBasedTuitionPartnerCompareListStorageService(
        IHttpContextAccessor httpContextAccessor, ILogger<CookieBasedTuitionPartnerCompareListStorageService> logger)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException($"{nameof(httpContextAccessor)}");
        _logger = logger ?? throw new ArgumentNullException($"{nameof(logger)}");
    }

    /// <summary>
    /// Adds a compare listed Tuition Partner to an implemented form of storage.
    /// </summary>
    /// <param name="compareListedTuitionPartnersSeoUrl"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public bool AddTuitionPartners(IEnumerable<string> compareListedTuitionPartnersSeoUrl)
    {
        compareListedTuitionPartnersSeoUrl = compareListedTuitionPartnersSeoUrl.ToList();
        var isFaultyData =
            compareListedTuitionPartnersSeoUrl.Any(seoUrl => !IsCompareListedTuitionPartnerSeoUrlValid(seoUrl));

        if (isFaultyData)
            throw new InvalidOperationException(
                $"One or more of the values in {nameof(compareListedTuitionPartnersSeoUrl)} is invalid");
        if (_httpContextAccessor.HttpContext == null) throw new ArgumentException($"{nameof(_httpContextAccessor.HttpContext)} is null");

        var isCallSuccessful = AddTuitionPartnersToCookie(compareListedTuitionPartnersSeoUrl);

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
    public bool RemoveTuitionPartner(string compareListedTuitionPartnerSeoUrl)
    {
        var result = false;
        compareListedTuitionPartnerSeoUrl = compareListedTuitionPartnerSeoUrl.Trim();
        var valuesInCookie = GetAllTuitionPartners().ToList();

        if (!valuesInCookie.Contains(compareListedTuitionPartnerSeoUrl, StringComparer.OrdinalIgnoreCase)) return result;

        valuesInCookie.RemoveAll(v => v.Equals(compareListedTuitionPartnerSeoUrl, StringComparison.OrdinalIgnoreCase));

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
            _logger.LogError(ex, "An error occurred while trying to delete the cookie {cookieName}.", CookieName);
            return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public bool IsTuitionPartnerCompareListed(string tuitionPartnerSeoUrl)
    {
        if (!IsCompareListedTuitionPartnerSeoUrlValid(tuitionPartnerSeoUrl))
            throw new InvalidOperationException($"{nameof(tuitionPartnerSeoUrl)} is invalid");

        var allCompareListedTps = GetAllTuitionPartners().ToList();
        return allCompareListedTps.Any(tp => tp == tuitionPartnerSeoUrl.Trim());
    }

    private bool IsCompareListedTuitionPartnerSeoUrlValid(string tuitionPartnerSeoUrl)
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
            _logger.LogError(ex, "An error occurred while trying to add the cookie {cookieName}.", CookieName);
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