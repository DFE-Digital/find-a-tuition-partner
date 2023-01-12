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
    public async void With_a_blank_postcode()
    {
        // Given
        await Fixture.AddTuitionPartner(A.TuitionPartner);

        // When
        var result = await Fixture.SendAsync(Basic.SearchResultsQuery);

        // Then
        new TestValidationResult<SearchResults.Query>(result.Validation)
            .ShouldNotHaveValidationErrorFor(x => x.Postcode);

        result.Results!.Results.Should().NotBeEmpty();
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

                page.Data.TuitionType = UI.Enums.TuitionType.Online;
                page.Data.OrganisationGroupingType = UI.Enums.OrganisationGroupingType.Charity;

                await page.OnGetClearAllFilters(postcode);

                return page;
            });

        result.Data.Postcode.Should().Be(postcode);
        result.Data.KeyStages.Should().BeNull();
        result.Data.Subjects.Should().BeNull();
        result.Data.TuitionType.Should().Be(UI.Enums.TuitionType.Any);
        result.Data.OrganisationGroupingType.Should().Be(UI.Enums.OrganisationGroupingType.Any);
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
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a", "Alpha")
            .TaughtIn(District.Dacorum, TuitionTypes.InSchool)
            .WithSubjects(s => s
                .Subject(Subjects.Id.KeyStage1English, l => l
                    .InSchool().Costing(12m).ForGroupSizes(2))));

        // When
        var result = await Fixture.SendAsync(
            Basic.SearchResultsQuery with { Postcode = District.Dacorum.SamplePostcode });

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
        // When
        var result = await Fixture.SendAsync(
            Basic.SearchResultsQuery with { Postcode = District.Dacorum.SamplePostcode });

        // Then
        result?.Results?.LocalAuthorityName.Should().Be("Hertfordshire");
    }

    [Fact]
    public async Task Preserves_selected_from_querystring()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner);

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

    [Fact]
    public async Task Verify_Search_Results_Count()
    {
        const int numberOfTuitionPartners = 55;

        // Given
        for (int tuitionPartnersAdded = 1; tuitionPartnersAdded <= numberOfTuitionPartners; tuitionPartnersAdded++)
            await Fixture.AddTuitionPartner(A.TuitionPartner);

        // When
        var result = await Fixture.SendAsync(new SearchResults.Query());

        // Then
        result!.Results.Should().NotBeNull();
        result!.Results!.Count.Should().Be(numberOfTuitionPartners);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("image-data", true)]
    public async Task Results_show_logos(string? logo, bool hasLogo)
    {
        // Given
        var partner = logo == null
            ? A.TuitionPartner
            : A.TuitionPartner.WithLogo(logo);
        await Fixture.AddTuitionPartner(partner);

        // When
        var result = await Fixture.SendAsync(Basic.SearchResultsQuery);

        // Then
        result!.Results!.Results.Should().ContainEquivalentOf(new
        {
            HasLogo = hasLogo,
        });
    }
}