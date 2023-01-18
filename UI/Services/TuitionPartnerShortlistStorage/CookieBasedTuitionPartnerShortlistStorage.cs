namespace UI.Services.TuitionPartnerShortlistStorage;

/// <summary>
/// The CookieBasedTuitionPartnerShortlistStorage stores ShortlistedTuitionPartner details in a cookie
/// for 24 hours/one day from the time it was added.
/// </summary>
public class CookieBasedTuitionPartnerShortlistStorage : ITuitionPartnerShortlistStorage
{
    // Name keys : Tps => TuitionPartners,Tp => TuitionPartner
    private const string CookieName = ".FindATuitionPartner.Shortlist";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CookieBasedTuitionPartnerShortlistStorage> _logger;

    public CookieBasedTuitionPartnerShortlistStorage(
        IHttpContextAccessor httpContextAccessor, ILogger<CookieBasedTuitionPartnerShortlistStorage> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <inheritdoc/>
    public void AddTuitionPartner(string shortlistedTuitionPartnerSeoUrl)
    {
        if (!IsShortlistedTuitionPartnerSeoUrlValid(shortlistedTuitionPartnerSeoUrl))
            throw new ArgumentException($"{nameof(shortlistedTuitionPartnerSeoUrl)} is invalid");
        if (_httpContextAccessor.HttpContext == null) throw GetHttpContextException();

        if (GetAllTuitionPartners().ToList().Contains(shortlistedTuitionPartnerSeoUrl.Trim())) return;

        var encodedTuitionPartnerSeoUrl = EncodeShortlistedTuitionPartnerSeoUrl(shortlistedTuitionPartnerSeoUrl);
        AddTuitionPartnerToCookie(CookieName, encodedTuitionPartnerSeoUrl);
    }

    /// <summary>
    /// Adds a Shortlisted Tuition Partner to an implemented form of storage.
    /// Note: The value in the cookie is replaced by the new values passed.
    /// </summary>
    /// <param name="shortlistedTuitionPartnersSeoUrls"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public void AddTuitionPartners(IEnumerable<string> shortlistedTuitionPartnersSeoUrls)
    {
        shortlistedTuitionPartnersSeoUrls = shortlistedTuitionPartnersSeoUrls.ToList();
        var isFaultyData =
            shortlistedTuitionPartnersSeoUrls.Any(seoUrl => !IsShortlistedTuitionPartnerSeoUrlValid(seoUrl));

        if (isFaultyData)
            throw new ArgumentException(
                $"One or more of the values in {nameof(shortlistedTuitionPartnersSeoUrls)} is invalid");
        if (_httpContextAccessor.HttpContext == null) throw GetHttpContextException();

        var encodedShortlistedTuitionPartnersSeoUrls =
            GetEncodedTuitionPartnersSeoUrls(shortlistedTuitionPartnersSeoUrls);

        AddTuitionPartnersToCookie(CookieName, encodedShortlistedTuitionPartnersSeoUrls);
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
    public void RemoveTuitionPartner(string shortlistedTuitionPartnerSeoUrl)
    {
        shortlistedTuitionPartnerSeoUrl = shortlistedTuitionPartnerSeoUrl.Trim();
        var valuesInCookie = GetAllTuitionPartners().ToList();

        if (!valuesInCookie.Contains(shortlistedTuitionPartnerSeoUrl)) return;

        valuesInCookie.RemoveAll(v => v == shortlistedTuitionPartnerSeoUrl);
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(CookieName);

        var encodedTuitionPartnersSeoUrls = GetEncodedTuitionPartnersSeoUrls(valuesInCookie).ToList();
        AddTuitionPartnersToCookie(CookieName, encodedTuitionPartnersSeoUrls);
    }

    ///<inheritdoc/>>
    public void RemoveAllTuitionPartners() =>
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(CookieName);

    /// <inheritdoc/>
    public bool IsTuitionPartnerShortlisted(string tuitionPartnerSeoUrl)
    {
        if (!IsShortlistedTuitionPartnerSeoUrlValid(tuitionPartnerSeoUrl))
            throw new ArgumentException($"{nameof(tuitionPartnerSeoUrl)} is invalid");

        var allShortlistedTps = GetAllTuitionPartners().ToList();
        return allShortlistedTps.Any(tp => tp == tuitionPartnerSeoUrl.Trim());
    }

    private bool IsShortlistedTuitionPartnerSeoUrlValid(string tuitionPartnerSeoUrl)
    {
        if (!string.IsNullOrEmpty(tuitionPartnerSeoUrl)) return true;

        _logger.LogError($"An invalid {nameof(tuitionPartnerSeoUrl)} provided");

        return false;
    }

    private string EncodeShortlistedTuitionPartnerSeoUrl(string shortlistedTuitionPartnerSeoUrl) =>
        Uri.EscapeDataString(shortlistedTuitionPartnerSeoUrl.Trim());

    private void AddTuitionPartnerToCookie(string cookieName, string encodedTuitionPartnerSeoUrl)
    {
        if (!IsCookiePresent(cookieName))
        {
            _httpContextAccessor.HttpContext?.Response.Cookies
                .Append(cookieName, encodedTuitionPartnerSeoUrl, GetCookieOptions());
        }
        else
        {
            var cookieValue = _httpContextAccessor.HttpContext?.Request.Cookies[$"{cookieName}"];
            cookieValue += string.IsNullOrWhiteSpace(cookieValue)
                ? $"{encodedTuitionPartnerSeoUrl}"
                : $"&{encodedTuitionPartnerSeoUrl}";
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(cookieName, cookieValue, GetCookieOptions());
        }
    }

    private bool IsCookiePresent(string cookieName) =>
        _httpContextAccessor.HttpContext != null &&
        _httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(cookieName);

    private IEnumerable<string> GetEncodedTuitionPartnersSeoUrls(IEnumerable<string> shortlistedTuitionPartnersSeoUrls)
        => shortlistedTuitionPartnersSeoUrls.Select(EncodeShortlistedTuitionPartnerSeoUrl);

    private ArgumentNullException GetHttpContextException() =>
        new($"{nameof(_httpContextAccessor.HttpContext)}");

    private void AddTuitionPartnersToCookie(string cookieName, IEnumerable<string> encodedTuitionPartnersSeoUrls)
    {
        encodedTuitionPartnersSeoUrls = encodedTuitionPartnersSeoUrls.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var cookieValueToAdd = new StringBuilder(encodedTuitionPartnersSeoUrls.Count());
        var counter = 0;
        foreach (var seoUrl in encodedTuitionPartnersSeoUrls)
        {
            cookieValueToAdd.Append(counter < encodedTuitionPartnersSeoUrls.Count() - 1
                ? $"{seoUrl}&"
                : $"{seoUrl}");
            counter++;
        }

        if (string.IsNullOrWhiteSpace(cookieValueToAdd.ToString()))
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(cookieName);

        _httpContextAccessor.HttpContext?.Response.Cookies
            .Append(cookieName, $"{cookieValueToAdd}", GetCookieOptions());
    }

    private static CookieOptions GetCookieOptions() => new() { Expires = DateTime.Now.AddDays(1), HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict };

    private IEnumerable<string> GetTuitionPartnersSeoUrlFromCookie(string cookie)
    {
        var split = cookie.Split("&", StringSplitOptions.RemoveEmptyEntries).ToList();

        return split.Select(Uri.UnescapeDataString).ToList();
    }
}