using UI.Pages.FindATuitionPartner;

namespace Tests.TestData;

public static class Basic
{
    public static SearchResults.Query SearchResultsQuery => new()
    {
        Postcode = "AA00AA",
        Subjects = new[] { "KeyStage1-English" },
    };
}
