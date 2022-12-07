namespace Application.TuitionPartnerShortlistStorage.Implementations;

/// <summary>
/// The CookieBasedTuitionPartnerShortlistStorage stores ShortlistedTuitionPartner details in a cookie
/// for 24 hours/one day from the time it was added.
/// </summary>
public class CookieBasedTuitionPartnerShortlistStorage : ITuitionPartnerShortlistStorage
{
    private const string CookieKeyFormat = "tp-cookie-seoUrl-";
    private const string ContainsBracket = "-containsBrackets";
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly ILogger<CookieBasedTuitionPartnerShortlistStorage> _logger;
    // private int _totalShortlistedTuitionPartners;

    public CookieBasedTuitionPartnerShortlistStorage(
        IHttpContextAccessor httpContextAccessor, ILogger<CookieBasedTuitionPartnerShortlistStorage> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        // _totalShortlistedTuitionPartners = GetAllTuitionPartners().Count();
    }

    /// <inheritdoc/>
    public int AddTuitionPartner(ShortlistedTuitionPartner shortlistedTuitionPartner)
    {
        if (!IsTuitionPartnerValid(shortlistedTuitionPartner)) return 0;
        if (_httpContextAccessor.HttpContext == null) return 0;
        var cookieKey = ConstructCookieKey(shortlistedTuitionPartner.SeoUrl,
            shortlistedTuitionPartner.LocalAuthorityName.Trim());
        if (IsTuitionPartnerPresent(cookieKey)) RemoveCookie(cookieKey);

        var stringifyTpDetail = StringifyTuitionPartnerDetail(shortlistedTuitionPartner);
        AddTuitionPartnerToCookie(cookieKey, stringifyTpDetail);
        // _totalShortlistedTuitionPartners++;
        return 1;
    }

    /// <inheritdoc/>
    public IEnumerable<ShortlistedTuitionPartner> GetTuitionPartnersByLocalAuthorityName(string localAuthorityName) =>
        GetAllTuitionPartners().Where(stp => stp.LocalAuthorityName == localAuthorityName.Trim());

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
    public int RemoveTuitionPartner(string seoUrl, string localAuthorityName)
    {
        RemoveCookie(ConstructCookieKey(seoUrl, localAuthorityName));
        // _totalShortlistedTuitionPartners = _totalShortlistedTuitionPartners--;

        return 1;
    }

    ///<inheritdoc/>>
    public int RemoveAllTuitionPartners()
    {
        var cookies = GetAllTuitionPartnerCookies();
        if (cookies == null) return 1;

        foreach (var cookie in cookies)
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(cookie.Key);

        // _totalShortlistedTuitionPartners = 0;

        return 1;
    }

    ///<inheritdoc/>>
    public int RemoveAllTuitionPartnersByLocalAuthority(string localAuthorityName)
    {
        var totalTuitionPartnersRemoved = 0;
        var shortlistedTuitionPartners = GetTuitionPartnersByLocalAuthorityName(localAuthorityName).ToList();

        if (!shortlistedTuitionPartners.Any()) return totalTuitionPartnersRemoved;

        foreach (var stp in shortlistedTuitionPartners)
        {
            RemoveTuitionPartner(stp.SeoUrl, stp.LocalAuthorityName);
            totalTuitionPartnersRemoved++;
        }

        return totalTuitionPartnersRemoved;
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

        if (!string.IsNullOrEmpty(shortlistedTuitionPartner.LocalAuthorityName)) return errorCount == 0;

        _logger.LogError(
            $"An invalid {nameof(ShortlistedTuitionPartner)}.{nameof(shortlistedTuitionPartner.LocalAuthorityName)} was provided");

        return false;
    }

    private string ConstructCookieKey(string seoUrl, string localAuthorityName)
    {
        char[] brackets = { '(', '[', '{', '}', ']', ')' };
        if (seoUrl.IndexOfAny(brackets) >= 0)
            seoUrl = $"{ReplaceBracket(seoUrl)}{ContainsBracket}";

        return $"{CookieKeyFormat}{seoUrl}-{localAuthorityName}";
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
        $"{shortlistedTuitionPartner.SeoUrl}&{shortlistedTuitionPartner.LocalAuthorityName.Trim()}";

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