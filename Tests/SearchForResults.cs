using Application.Exceptions;
using Domain;
using Domain.Constants;
using Domain.Search;
using FluentValidation.TestHelper;
using NSubstitute;
using UI.Pages.FindATuitionPartner;
using Index = UI.Pages.FindATuitionPartner.Index;
using KeyStage = UI.Pages.FindATuitionPartner.KeyStage;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class SearchForResults : CleanSliceFixture
{
    public SearchForResults(SliceFixture fixture) : base(fixture) { }

    [Theory]
    [InlineData("not a postcode")]
    public void With_an_invalid_postcode(string postcode)
    {
        var model = new Index.Command { Postcode = postcode };
        var result = new Index.Validator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Postcode)
            .WithErrorMessage("Enter a valid postcode");
    }

    [Theory]
    [InlineData("AA0 0AA")]
    public async void With_a_postcode_that_is_not_found(string postcode)
    {
        Fixture.LocationFilter.GetLocationFilterParametersAsync(postcode)
            .Returns(null as LocationFilterParameters);

        var result = await Fixture.SendAsync(new SearchResults.Query
        {
            Postcode = postcode,
            Subjects = new[] { "KeyStage1-English" },
        });

        var validationResult = new TestValidationResult<SearchResults.Query>(result.Validation);
        validationResult.ShouldHaveValidationErrorFor(x => x.Postcode)
            .WithErrorMessage("Enter a valid postcode");
    }

    [Fact]
    public async Task Displays_all_tutor_types_in_database()
    {
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
                        SubjectId = Subjects.Id.KeyStage1English
                    }
                }
            });

            await db.SaveChangesAsync();
        });

        var subject = await Fixture.ExecuteDbContextAsync(db =>
            db.Subjects.FindAsync(Subjects.Id.KeyStage1English));

        var result = await Fixture.SendAsync(new SearchResults.Query
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
                        SubjectId = Subjects.Id.KeyStage1English
                    }
                }
            });

            await db.SaveChangesAsync();
        });

        var subject = await Fixture.ExecuteDbContextAsync(db =>
            db.Subjects.FindAsync(Subjects.Id.KeyStage1English));

        var result = await Fixture.SendAsync(new SearchResults.Query
        {
            Postcode = "AB00BA",
            KeyStages = new[] { KeyStage.KeyStage1 },
            Subjects = new[] { $"{KeyStage.KeyStage1}-{subject?.Name}" }
        });

        result.Results.Should().NotBeNull();
        result.AllSubjects.Values.SelectMany(x => x)
            .Where(x => x.Selected).Should().BeEquivalentTo(new[]
            {
                new { Name = "English", Selected = true },
            });
    }
}