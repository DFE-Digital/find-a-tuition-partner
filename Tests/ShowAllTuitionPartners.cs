using Tests.TestData;
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
        // Arrange
        await Fixture.AddTuitionPartner(A.TuitionPartner.WithName("Beta"));
        await Fixture.AddTuitionPartner(A.TuitionPartner.WithName("Gamma"));
        await Fixture.AddTuitionPartner(A.TuitionPartner.WithName("Alpha"));

        // Act
        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;
        var page = await Fixture.GetPage<AllTuitionPartners>(page => page.OnGet(cancellationToken));

        // Assert
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

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;
        var page = await Fixture.GetPage<AllTuitionPartners>(page => page.OnGet(cancellationToken));

        page.Results!.Results.Should().BeEquivalentTo(new[]
        {
            new { Name = "Alpha", HasLogo = true },
            new { Name = "Bravo", HasLogo = false },
        });
    }

    [Fact]
    public async Task Search_by_name()
    {
        // Arrange
        await Fixture.AddTuitionPartner(A.TuitionPartner.WithName("Alpha"));
        await Fixture.AddTuitionPartner(A.TuitionPartner.WithName("Beta"));

        // Act
        var page = await Fixture.GetPage<AllTuitionPartners>(page =>
        {
            page.Data.Name = "LPh";
            CancellationTokenSource cts = new();
            CancellationToken cancellationToken = cts.Token;
            return page.OnGet(cancellationToken);
        });

        // Assert
        page.Results.Should().NotBeNull();
        page.Results!.Results.Should().BeEquivalentTo(new[]
        {
            new { Name = "Alpha" }
        });
    }
}