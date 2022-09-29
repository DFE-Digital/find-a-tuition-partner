using Domain.Constants;
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
        await Fixture.ExecuteDbContextAsync(async db =>
        {
            db.TuitionPartners.Add(new Domain.TuitionPartner
            {
                Name = "Beta",
                SeoUrl = "beta",
                Website = "http://"
            });

            db.TuitionPartners.Add(new Domain.TuitionPartner
            {
                Name = "Gamma",
                SeoUrl = "gamma",
                Website = "http://"
            });

            db.TuitionPartners.Add(new Domain.TuitionPartner
            {
                Name = "Alpha",
                SeoUrl = "alpha",
                Website = "http://"
            });

            await db.SaveChangesAsync();
        });

        // When
        var page = await Fixture.GetPage<FullList>()
            .Execute(async page =>
            {
                await page.OnGet();
                return page;
            });

        // Then
        page.Results.Should().NotBeNull();
        page.Results!.Results.Should().BeEquivalentTo(new[]
        {
            new { Name = "Alpha" },
            new { Name = "Beta" },
            new { Name = "Gamma" }
        }, options => options.WithStrictOrdering());
    }
}