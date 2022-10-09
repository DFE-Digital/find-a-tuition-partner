using UI.Pages;

namespace Tests.TestData;

public static class Basic
{
    public static SearchResults.Query SearchResultsQuery => new()
    {
        Postcode = "AA00AA",
        KeyStages = new[] { KeyStage.KeyStage1 },
        Subjects = new[] { "KeyStage1-English" },
    };
}

public static class A
{
    public static TuitionPartnerBuilder TuitionPartner => new();
}
