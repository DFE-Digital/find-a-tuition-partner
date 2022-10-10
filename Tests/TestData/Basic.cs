using Domain.Constants;
using UI.Pages;

namespace Tests.TestData;

public static class Basic
{
    public static SearchResults.Query SearchResultsQuery => new()
    {
        KeyStages = new[] { KeyStage.KeyStage1 },
        Subjects = new[] { "KeyStage1-English" },
    };
}

public static class A
{
    public static TuitionPartnerBuilder TuitionPartner => new TuitionPartnerBuilder()
        .WithSubjects(s => s
            .Subject(Subjects.Id.KeyStage1English, l => l
                .InSchool()
                .Costing(12m)
                .ForGroupSizes(2)));
}
