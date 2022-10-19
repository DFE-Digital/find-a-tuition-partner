using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Tests.TestData;
using UI.Extensions;
using UI.Pages;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class ShowAllTuitionPartners : CleanSliceFixture
{
    public ShowAllTuitionPartners(SliceFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Displays_all_tuition_partners_in_database_in_alphabetical_ordering()
    {
        // Given
        await Fixture.AddTuitionPartner(A.TuitionPartner.WithName("Beta"));
        await Fixture.AddTuitionPartner(A.TuitionPartner.WithName("Gamma"));
        await Fixture.AddTuitionPartner(A.TuitionPartner.WithName("Alpha"));

        // When
        var page = await Fixture.GetPage<AllTuitionPartners>(page => page.OnGet());

        // Then
        page.Results.Should().NotBeNull();
        page.Results!.Results.Should().BeEquivalentTo(new[]
        {
            new { Name = "Alpha" },
            new { Name = "Beta" },
            new { Name = "Gamma" }
        }, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Displays_logos_when_available()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner.WithName("Alpha").WithLogo("alpha-logo"));
        await Fixture.AddTuitionPartner(A.TuitionPartner.WithName("Bravo"));

        var page = await Fixture.GetPage<AllTuitionPartners>(page => page.OnGet());

        page.Results!.Results.Should().BeEquivalentTo(new[]
        {
            new { Name = "Alpha", HasLogo = true },
            new { Name = "Bravo", HasLogo = false },
        });
    }

    [Fact]
    public async Task Search_by_name()
    {
        // Given
        await Fixture.AddTuitionPartner(A.TuitionPartner.WithName("Alpha"));
        await Fixture.AddTuitionPartner(A.TuitionPartner.WithName("Beta"));

        // When
        var page = await Fixture.GetPage<AllTuitionPartners>(page =>
        {
            page.Data.Name = "LPh";
            return page.OnGet();
        });

        // Then
        page.Results.Should().NotBeNull();
        page.Results!.Results.Should().BeEquivalentTo(new[]
        {
            new { Name = "Alpha" }
        });
    }

    [Fact]
    public async Task Sets_from_full_list()
    {
        var page = await Fixture.GetPage<AllTuitionPartners>(page =>
        {
            page.Data.From = ReferrerList.SearchResults;
            return page.OnGet();
        });

        page.Data.From.Should().Be(ReferrerList.FullList);
    }

    [Fact]
    public async Task Sets_AllSearchData()
    {
        var page = await Fixture.GetPage<AllTuitionPartners>(page =>
        {
            page.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            page.Data.From = ReferrerList.SearchResults;
            page.Data.Name = "test";
            return page.OnGet();
        });

        var data = page.TempData.Peek<SearchModel>("AllSearchData");
        data.Should().NotBeNull();
        data!.From.Should().Be(ReferrerList.FullList);
        data.Name.Should().Be("test");
        data.Postcode.Should().BeNull();
        data.Subjects.Should().BeNull();
        data.TuitionType.Should().BeNull();
        data.KeyStages.Should().BeNull();
    }
}