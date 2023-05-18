using Application.Queries;
using Domain.Constants;
using Domain.Enums;

namespace Tests.TestData;

public static class Basic
{
    public static GetSearchResultsQuery SearchResultsQuery => new()
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
                .FaceToFace()
                .Costing(12m)
                .ForGroupSizes(2)));
}
