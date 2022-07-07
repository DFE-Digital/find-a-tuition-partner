using Domain;
using Domain.Constants;
using Domain.Search;
using NSubstitute;
using UI.Pages.FindATuitionPartner;
using KeyStage = UI.Pages.FindATuitionPartner.KeyStage;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class SearchForResults : CleanSliceFixture
{
    public SearchForResults(SliceFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Displays_all_tutor_types_in_database()
    {
        Fixture.LocationFilter.GetLocationFilterParametersAsync(Arg.Any<string>())
            .Returns(new LocationFilterParameters { LocalAuthorityDistrictCode = "N1" });

        await Fixture.ExecuteDbContextAsync(async db =>
        {
            db.TuitionPartners.Add(new Domain.TuitionPartner
            {
                Name = "Alpha",
                SeoUrl = "a",
                Website = "-",
                SubjectCoverage = new List<SubjectCoverage>
                {
                    new()
                    {
                        TuitionType = db.TuitionTypes.First(),
                        SubjectId = Subjects.Id.KeyStage1Literacy
                    }
                }
            });

            await db.SaveChangesAsync();
        });

        var subject = await Fixture.ExecuteDbContextAsync(db =>
            db.Subjects.FindAsync(Subjects.Id.KeyStage1Literacy));

        var result = await Fixture.SendAsync(new SearchResults.Command
        {
            Postcode = "AB00BA",
            Subjects = new[] { $"{KeyStage.KeyStage1}-{subject?.Name}" }
        });

        result.Results.Should().NotBeNull();
        result.Results!.Results.Should().BeEquivalentTo(new[]
        {
            new { Name = "Alpha" },
        });
    }
    
    [Fact]
    public async Task Preserves_selected_from_querystring()
    {
        Fixture.LocationFilter.GetLocationFilterParametersAsync(Arg.Any<string>())
            .Returns(new LocationFilterParameters { LocalAuthorityDistrictCode = "N1" });

        await Fixture.ExecuteDbContextAsync(async db =>
        {
            db.TuitionPartners.Add(new Domain.TuitionPartner
            {
                Name = "Beta",
                SeoUrl = "b",
                Website = "-",
                SubjectCoverage = new List<SubjectCoverage>
                {
                    new()
                    {
                        TuitionType = db.TuitionTypes.First(),
                        SubjectId = Subjects.Id.KeyStage1Literacy
                    }
                }
            });

            await db.SaveChangesAsync();
        });

        var subject = await Fixture.ExecuteDbContextAsync(db =>
            db.Subjects.FindAsync(Subjects.Id.KeyStage1Literacy));

        var result = await Fixture.SendAsync(new SearchResults.Command
        {
            Postcode = "AB00BA",
            KeyStages = new[] { KeyStage.KeyStage1 },
            Subjects = new[] { $"{KeyStage.KeyStage1}-{subject?.Name}" }
        });

        result.Results.Should().NotBeNull();
        result.AllSubjects.Values.SelectMany(x => x)
            .Where(x => x.Selected).Should().BeEquivalentTo(new[]
            {
                new { Name = "Literacy", Selected = true },
            });
    }
}