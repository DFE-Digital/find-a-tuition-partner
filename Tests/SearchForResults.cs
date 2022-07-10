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
    public async Task Displays_all_subjects_in_key_stage_after_validation_failure()
    {
        var query = new SearchResults.Query
        {
            Subjects = null,
            KeyStages = new[] { KeyStage.KeyStage1 }
        };

        var result = await Fixture.SendAsync(query);

        result.AllSubjects.Should().HaveCount(4);

        result.AllSubjects.Should().ContainKey(KeyStage.KeyStage1)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "Maths" },
                new { Name = "English" },
                new { Name = "Science" },
            });

        result.AllSubjects.Should().ContainKey(KeyStage.KeyStage2)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "Maths" },
                new { Name = "English" },
                new { Name = "Science" },
            });

        result.AllSubjects.Should().ContainKey(KeyStage.KeyStage3)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "Maths" },
                new { Name = "English" },
                new { Name = "Science" },
                new { Name = "Humanities" },
                new { Name = "Modern foreign languages" },
            });
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
        result.AllSubjects.Should().ContainKey(KeyStage.KeyStage1)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "English", Selected = true },
                new { Name = "Maths", Selected = false },
                new { Name = "Science", Selected = false },
            });
    }
}