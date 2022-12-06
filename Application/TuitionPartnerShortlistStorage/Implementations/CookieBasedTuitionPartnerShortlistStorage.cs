using Application.TuitionPartnerShortlistStorage.Interfaces;
using Domain.Search.ShortlistTuitionPartners;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.TuitionPartnerShortlistStorage.Implementations;

/// <summary>
/// The CookieBasedTuitionPartnerShortlistStorage stores ShortlistedTuitionPartner details in a cookie
/// for 24 hours / one day.
/// </summary>
public class CookieBasedTuitionPartnerShortlistStorage : ITuitionPartnerShortlistStorage
{
    private const string CookieKeyFormat = "tp-cookie-seoUrl-";
    private const string ContainsBracket = "-containsBrackets";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CookieBasedTuitionPartnerShortlistStorage> _logger;
    private int _totalShortlistedTuitionPartners;

    public CookieBasedTuitionPartnerShortlistStorage(
        IHttpContextAccessor httpContextAccessor, ILogger<CookieBasedTuitionPartnerShortlistStorage> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _totalShortlistedTuitionPartners = GetAllTuitionPartners().Count();
    }

    /// <inheritdoc/>
    public int AddTuitionPartner(ShortlistedTuitionPartner shortlistedTuitionPartner)
    {
        if (!IsTuitionPartnerValid(shortlistedTuitionPartner)) return 0;
        if (_httpContextAccessor.HttpContext == null) return 0;
        var cookieKey = ConstructCookieKey(shortlistedTuitionPartner.SeoUrl,
            shortlistedTuitionPartner.LocalAuthority.Trim());
        if (IsTuitionPartnerPresent(cookieKey)) RemoveCookie(cookieKey);

        var stringifyTpDetail = StringifyTuitionPartnerDetail(shortlistedTuitionPartner);
        AddTuitionPartnerToCookie(cookieKey, stringifyTpDetail);
        _totalShortlistedTuitionPartners++;
        return 1;
    }

    /// <inheritdoc/>
    public IEnumerable<ShortlistedTuitionPartner> GetTuitionPartnersByLocalAuthorityName(string localAuthority) =>
        GetAllTuitionPartners().Where(stp => stp.LocalAuthority == localAuthority.Trim());

    ///<inheritdoc/>
    public IEnumerable<ShortlistedTuitionPartner> GetAllTuitionPartners()
    {
        var tuitionPartners = new List<ShortlistedTuitionPartner>();
        var cookies = GetAllTuitionPartnerCookies();
        if (cookies == null) return tuitionPartners;

        foreach (var cookie in cookies)
        {
            var tuitionPartner = GetTuitionPartnerFromCookie(cookie.Value);
            if (tuitionPartner != null) tuitionPartners.Add(tuitionPartner);
        }

        return tuitionPartners;
    }

    ///<inheritdoc />
    public int RemoveTuitionPartner(string seoUrl, string localAuthority)
    {
        // seoUrl = RetrieveValueFromKey(seoUrl);
        RemoveCookie(ConstructCookieKey(seoUrl, localAuthority));
        _totalShortlistedTuitionPartners = _totalShortlistedTuitionPartners--;

        return 1;
    }

    ///<inheritdoc/>>
    public int RemoveAllTuitionPartners()
    {
        var cookies = GetAllTuitionPartnerCookies();
        if (cookies == null) return 1;

        foreach (var cookie in cookies)
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(cookie.Key);

        _totalShortlistedTuitionPartners = 0;

        return 1;
    }

    private bool IsTuitionPartnerValid(ShortlistedTuitionPartner shortlistedTuitionPartner)
    {
        var errorCount = 0;
        if (string.IsNullOrEmpty(shortlistedTuitionPartner.SeoUrl))
        {
            errorCount += 1;
            _logger.LogError(
                $"An invalid {nameof(ShortlistedTuitionPartner)}.{nameof(shortlistedTuitionPartner.SeoUrl)} was provided");
        }

        if (!string.IsNullOrEmpty(shortlistedTuitionPartner.LocalAuthority)) return errorCount == 0;

        _logger.LogError(
            $"An invalid {nameof(ShortlistedTuitionPartner)}.{nameof(shortlistedTuitionPartner.LocalAuthority)} was provided");

        return false;
    }

    private string ConstructCookieKey(string seoUrl, string localAuthority)
    {
        char[] brackets = { '(', '[', '{', '}', ']', ')' };
        if (seoUrl.IndexOfAny(brackets) >= 0)
            seoUrl = $"{ReplaceBracket(seoUrl)}{ContainsBracket}";

        return $"{CookieKeyFormat}{seoUrl}-{localAuthority}";
    }

    private string ReplaceBracket(string value) => value.Replace("(", "400028")
        .Replace(")", "410029").Replace("[", "91005B")
        .Replace("]", "93005D").Replace("{", "123007B")
        .Replace("}", "125007D");

    private bool IsTuitionPartnerPresent(string cookieName) =>
        _httpContextAccessor.HttpContext != null &&
        _httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(cookieName);

    private void RemoveCookie(string cookieKey) =>
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(cookieKey);

    private string StringifyTuitionPartnerDetail(ShortlistedTuitionPartner shortlistedTuitionPartner) =>
        $"{shortlistedTuitionPartner.SeoUrl}&{shortlistedTuitionPartner.LocalAuthority.Trim()}";

    private void AddTuitionPartnerToCookie(string cookieKey, string stringifyTpDetail)
    {
        var cookieOptions = new CookieOptions();
        cookieOptions.Expires = DateTime.Now.AddDays(1);
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(cookieKey, stringifyTpDetail, cookieOptions);
    }

    private IEnumerable<KeyValuePair<string, string>>? GetAllTuitionPartnerCookies() =>
        _httpContextAccessor.HttpContext?.Request.Cookies
            .Where(c => c.Key.Contains(CookieKeyFormat));

    private ShortlistedTuitionPartner? GetTuitionPartnerFromCookie(string cookie)
    {
        var tuitionPartnerDetails = cookie.Split("&");
        return new ShortlistedTuitionPartner(tuitionPartnerDetails[0], tuitionPartnerDetails[1]);
    }
}