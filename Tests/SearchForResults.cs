using Domain;
using Domain.Constants;
using Domain.Search;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NSubstitute;
using Tests.TestData;
using UI.Pages;
using Index = UI.Pages.Index;
using KeyStage = UI.Pages.KeyStage;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class SearchForResults : CleanSliceFixture
{
    public SearchForResults(SliceFixture fixture) : base(fixture)
    {
    }

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

        var result = await Fixture.SendAsync(
            Basic.SearchResultsQuery with { Postcode = postcode });

        var validationResult = new TestValidationResult<SearchResults.Query>(result.Validation);
        validationResult.ShouldHaveValidationErrorFor(x => x.Postcode)
            .WithErrorMessage("Enter a valid postcode");
    }

    [Theory]
    [InlineData("LL58 8EP", "Wales")]
    [InlineData("DD1 4NP", "Scotland")]
    public async void With_a_postcode_outside_of_England(string postcode, string country)
    {
        Fixture.LocationFilter.GetLocationFilterParametersAsync(postcode)
            .Returns(new LocationFilterParameters { Country = country });

        var result = await Fixture.SendAsync(
            Basic.SearchResultsQuery with { Postcode = postcode });

        var validationResult = new TestValidationResult<SearchResults.Query>(result.Validation);
        validationResult.ShouldHaveValidationErrorFor(x => x.Postcode)
            .WithErrorMessage("This service covers England only");
    }

    [Theory]
    [InlineData("LL58 8EP")]
    [InlineData("DD1 4NP")]
    public async void With_a_postcode_that_cannot_be_mapped(string postcode)
    {
        Fixture.LocationFilter.GetLocationFilterParametersAsync(postcode)
            .Returns(new LocationFilterParameters
            {
                Country = "England",
                LocalAuthorityDistrictCode = null,
            });

        var result = await Fixture.SendAsync(
            Basic.SearchResultsQuery with { Postcode = postcode });

        var validationResult = new TestValidationResult<SearchResults.Query>(result.Validation);
        validationResult.ShouldHaveValidationErrorFor(x => x.Postcode)
            .WithErrorMessage("Could not identify Local Authority for the supplied postcode");
    }

    [Fact]
    public async void With_a_blank_postcode()
    {
        // Given
        await SetupTuitionPartner();

        // When
        var result = await Fixture.SendAsync(
            Basic.SearchResultsQuery with { Postcode = "" });

        // Then
        new TestValidationResult<SearchResults.Query>(result.Validation)
            .ShouldNotHaveValidationErrorFor(x => x.Postcode);

        result.Results!.Results.Should().NotBeEmpty();
    }

    [Fact]
    public async void With_postcode_only_and_filters_null()
    {
        var result = await Fixture.GetPage<SearchResults>().
        Execute(page => {

        page.Data.KeyStages = new[]
        {
            KeyStage.KeyStage1
        };
            page.Data.Subjects = new[]
        {
            $"{KeyStage.KeyStage1}-English", $"{KeyStage.KeyStage1}-Humanities",
        };
            page.Data.TuitionType = UI.Pages.TuitionType.Online;
            return page.OnGetClearAllFilters("SK1 1EB");
        });
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task Displays_all_subjects_after_validation_failure()
    {
        var query = new SearchResults.Query
        {
            Subjects = null,
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

        result.AllSubjects.Should().ContainKey(KeyStage.KeyStage4)
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
        // Given
        await Fixture.ExecuteDbContextAsync(async db =>
        {
            var lad = await db.LocalAuthorityDistricts.FirstAsync(x => x.Code == "E07000096");
            var tuitionType = await db.TuitionTypes.FirstAsync();

            db.TuitionPartners.Add(new Domain.TuitionPartner
            {
                Name = "Alpha",
                SeoUrl = "a",
                Website = "-",
                LocalAuthorityDistrictCoverage = new List<LocalAuthorityDistrictCoverage>
                {
                    new()
                    {
                        TuitionType = tuitionType,
                        LocalAuthorityDistrict = lad,
                    }
                },
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

        // When
        var result = await Fixture.SendAsync(Basic.SearchResultsQuery);

        // Then
        result.Results.Should().NotBeNull();
        result.Results!.Results.Should().BeEquivalentTo(new[]
        {
            new { Name = "Alpha" },
        });
    }

    [Fact]
    public async Task Displays_local_authority()
    {
        // Given
        await SetupTuitionPartner();

        var subject = await Fixture.ExecuteDbContextAsync(db =>
            db.Subjects.FindAsync(Subjects.Id.KeyStage1English));

        // When
        var result = await Fixture.SendAsync(Basic.SearchResultsQuery);

        // Then
        result.LocalAuthority.Should().Be("Hertfordshire");
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

        var result = await Fixture.SendAsync(Basic.SearchResultsQuery);

        result.Results.Should().NotBeNull();
        result.AllSubjects.Should().ContainKey(KeyStage.KeyStage1)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "English", Selected = true },
                new { Name = "Maths", Selected = false },
                new { Name = "Science", Selected = false },
            });
    }

    private async Task SetupTuitionPartner()
    {
        await Fixture.ExecuteDbContextAsync(async db =>
        {
            var lad = await db.LocalAuthorityDistricts.FirstAsync(x => x.Code == "E07000096");
            var tuitionType = await db.TuitionTypes.FirstAsync();

            var tp = db.TuitionPartners.Add(new Domain.TuitionPartner
            {
                Name = "Alpha",
                SeoUrl = "a",
                Website = "-",
                LocalAuthorityDistrictCoverage = new List<LocalAuthorityDistrictCoverage>
                {
                    new()
                    {
                        TuitionType = tuitionType,
                        LocalAuthorityDistrict = lad,
                    }
                },
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
    }
}