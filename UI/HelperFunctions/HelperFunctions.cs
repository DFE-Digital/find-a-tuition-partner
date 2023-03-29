using UI.Constants;

namespace UI.HelperFunctions;

public static class HelperFunctions
{
    public const string JumpToLocationQueryName = "jumpToLocation";
    public static string GetJumpToLocation(HttpRequest request) =>
        !string.IsNullOrWhiteSpace(request.Query[JumpToLocationQueryName])
            ? $"#{request.Query[JumpToLocationQueryName]}"
            : "";

    public static string GetPageUrl(ReferrerList? referrerList, string defaultPageUrl = "")
    {
        return referrerList switch
        {
            ReferrerList.SearchResults => "search-results",
            ReferrerList.CompareList => "compare-list",
            _ => defaultPageUrl
        };
    }

    public static string GetPageName(ReferrerList? referrerList, string defaultPageName = "")
    {
        return referrerList switch
        {
            ReferrerList.SearchResults => "Back to search results",
            ReferrerList.CompareList => "Back to price comparison list",
            _ => defaultPageName
        };
    }

    public static double SessionTimeoutInSeconds()
    {
        return (DoubleConstants.SessionTimeoutInMinutes * 60);
    }

    public static double SessionCountdownMessageShownBeforeTimeoutInSeconds()
    {
        var sessionCountdownMessageShownBeforeTimeoutInSeconds = DoubleConstants.SessionCountdownMessageShownBeforeTimeoutInSeconds;
        return SessionTimeoutInSeconds() < sessionCountdownMessageShownBeforeTimeoutInSeconds ? SessionTimeoutInSeconds() : sessionCountdownMessageShownBeforeTimeoutInSeconds;
    }
}