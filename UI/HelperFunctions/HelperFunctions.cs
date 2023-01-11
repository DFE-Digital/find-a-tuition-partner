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
            ReferrerList.Shortlist => "shortlist",
            _ => defaultPageUrl
        };
    }

    public static string GetPageName(ReferrerList? referrerList, string defaultPageName = "")
    {
        return referrerList switch
        {
            ReferrerList.SearchResults => "Back to search results",
            ReferrerList.Shortlist => "Back to shortlist",
            _ => defaultPageName
        };
    }
}