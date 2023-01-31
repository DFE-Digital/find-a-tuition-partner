using Domain;
using Domain.Constants;
using Domain.Search;
using FluentValidation.TestHelper;
using NSubstitute;
using Tests.TestData;
using UI.Pages;
using Index = UI.Pages.Index;
using KeyStage = UI.Enums.KeyStage;

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
    public async void With_a_blank_postcode_should_have_postcode_validation_error()
    {
        // Arrange
        await Fixture.AddTuitionPartner(A.TuitionPartner);

        var searchResultsQuery = Basic.SearchResultsQuery;
        searchResultsQuery.Postcode = "";

        // Act
        var result = await Fixture.SendAsync(searchResultsQuery);

        // Assert
        PostCodeEmptyOrInvalidShouldHaveValidationError(result, "Enter a postcode");
    }

    [Fact]
    public async void Clear_filters_resets_all_bar_postcode()
    {
        const string postcode = "SK1 1EB";

        var result = await Fixture.GetPage<SearchResults>()
            .Execute(async page =>
            {
                page.Data.KeyStages = new[]
                {
                    KeyStage.KeyStage1
                };

                page.Data.Subjects = new[]
                {
                    $"{KeyStage.KeyStage1}-English", $"{KeyStage.KeyStage1}-Humanities",
                };

                page.Data.TuitionType = Domain.Enums.TuitionType.Online;

                await page.OnGetClearAllFilters(postcode);

                return page;
            });

        result.Data.Postcode.Should().Be(postcode);
        result.Data.KeyStages.Should().BeNull();
        result.Data.Subjects.Should().BeNull();
        result.Data.TuitionType.Should().Be(Domain.Enums.TuitionType.Any);
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
        // Arrange
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a", "Alpha")
            .TaughtIn(District.Dacorum, TuitionTypes.InSchool)
            .WithSubjects(s => s
                .Subject(Subjects.Id.KeyStage1English, l => l
                    .InSchool().Costing(12m).ForGroupSizes(2))));

        // Act
        var result = await Fixture.SendAsync(
            Basic.SearchResultsQuery with { Postcode = District.Dacorum.SamplePostcode });

        // Assert
        result.Results.Should().NotBeNull();
        result.Results!.Results.Should().BeEquivalentTo(new[]
        {
            new { Name = "Alpha" },
        });
    }

    [Fact]
    public async Task Displays_local_authority()
    {
        // Act
        var result = await Fixture.SendAsync(
            Basic.SearchResultsQuery with { Postcode = District.Dacorum.SamplePostcode });

        // Assert
        result?.Results?.LocalAuthorityName.Should().Be("Hertfordshire");
    }

    [Fact]
    public async Task Preserves_selected_from_querystring()
    {
        // Arrange
        await Fixture.AddTuitionPartner(A.TuitionPartner);

        var searchResultsQuery = Basic.SearchResultsQuery;
        searchResultsQuery.Postcode = District.Dacorum.SamplePostcode;

        // Act
        var result = await Fixture.SendAsync(searchResultsQuery);

        // Assert
        result.Results.Should().NotBeNull();
        result.AllSubjects.Should().ContainKey(KeyStage.KeyStage1)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "English", Selected = true },
                new { Name = "Maths", Selected = false },
                new { Name = "Science", Selected = false },
            });

        result.Postcode.Should().Be(District.Dacorum.SamplePostcode);
    }

    [Fact]
    public async Task Verify_Search_Results_Count_With_Valid_PostCode()
    {
        // Arrange
        const int numberOfTuitionPartners = 55;

        for (int tuitionPartnersAdded = 1; tuitionPartnersAdded <= numberOfTuitionPartners; tuitionPartnersAdded++)
            await Fixture.AddTuitionPartner(A.TuitionPartner.
                TaughtIn(District.EastRidingOfYorkshire, TuitionTypes.InSchool, TuitionTypes.Online));

        var searchResultsQuery = Basic.SearchResultsQuery;
        searchResultsQuery.Postcode = District.EastRidingOfYorkshire.SamplePostcode;

        // Act
        var result = await Fixture.SendAsync(searchResultsQuery);

        // Assert
        result!.Results.Should().NotBeNull();
        result!.Results!.Count.Should().Be(numberOfTuitionPartners);
    }

    [Fact]
    public async Task Verify_Search_Results_With_InValid_PostCode()
    {
        // Arrange
        const int numberOfTuitionPartners = 55;

        for (int tuitionPartnersAdded = 1; tuitionPartnersAdded <= numberOfTuitionPartners; tuitionPartnersAdded++)
            await Fixture.AddTuitionPartner(A.TuitionPartner.
                TaughtIn(District.EastRidingOfYorkshire, TuitionTypes.InSchool, TuitionTypes.Online));

        var searchResultsQuery = Basic.SearchResultsQuery;
        searchResultsQuery.Postcode = "AAAA BBCCD"; ;

        // Act
        var result = await Fixture.SendAsync(searchResultsQuery);

        // Assert
        result!.Results.Should().BeNull();
        PostCodeEmptyOrInvalidShouldHaveValidationError(result, "Enter a valid postcode");
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("image-data", true)]
    public async Task Results_show_logos(string? logo, bool hasLogo)
    {
        // Arrange
        var searchResultsQuery = Basic.SearchResultsQuery;
        searchResultsQuery.Postcode = District.EastRidingOfYorkshire.SamplePostcode;

        var partner = logo == null
            ? A.TuitionPartner.TaughtIn(District.EastRidingOfYorkshire, TuitionTypes.InSchool, TuitionTypes.Online)
            : A.TuitionPartner.WithLogo(logo).TaughtIn(District.EastRidingOfYorkshire, TuitionTypes.InSchool, TuitionTypes.Online);
        await Fixture.AddTuitionPartner(partner);

        // Act
        var result = await Fixture.SendAsync(searchResultsQuery);

        // Assert
        result!.Results!.Results.Should().ContainEquivalentOf(new
        {
            HasLogo = hasLogo,
        });
    }

    private static void PostCodeEmptyOrInvalidShouldHaveValidationError(SearchResults.ResultsModel result, string errorMessage)
    {
        result.Validation?.IsValid.Should().Be(false);

        result.Validation?.Errors.Count.Should().Be(1);

        result.Validation?.Errors[0].ErrorMessage.Should().Be(errorMessage);
    }
}