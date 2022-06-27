using Application.Handlers;
using Domain;
using Domain.Search;
using NSubstitute;
using UI.Pages.Find;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class SearchResults : IAsyncLifetime
{
    private readonly SliceFixture fixture;
    public SearchResults(SliceFixture fixture) => this.fixture = fixture;

    public Task InitializeAsync() => fixture.ResetDatabase();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Displays_all_tutor_types_in_database2()
    {
        await ResetDatabase();

        fixture.LocationFilter.GetLocationFilterParametersAsync(Arg.Any<string>())
            .Returns(new LocationFilterParameters { LocalAuthorityDistrictCode = "N1" });

        await fixture.ExecuteDbContextAsync(async db =>
        {
            db.Regions.AddRange(
                new Region { Code = "-", Name = "-", }
                );

            db.TuitionTypes.Add(new TuitionType { Name = "In Person" });

            db.Subjects.Add(new Subject { Name = "English" });

            await db.SaveChangesAsync();

            db.LocalAuthorityDistricts.AddRange(
                new LocalAuthorityDistrict
                { 
                    Code = "N1", 
                    Name = "North-One",
                    Region = db.Regions.First(),
                    LocalAuthority = new LocalAuthority { Code = "A", Name = "-", Region = db.Regions.First() },
                },
                new LocalAuthorityDistrict
                {
                    Code = "S2",
                    Name = "South-Two",
                    Region = db.Regions.First(),
                    LocalAuthority = new LocalAuthority { Code = "B", Name = "-", Region = db.Regions.First() }
                }
                );
            
            await db.SaveChangesAsync();

            db.TuitionPartners.Add(new TuitionPartner
            {
                Name = "Alpha",
                Website = "-",
                Coverage = new List<TuitionPartnerCoverage>
                {
                    new TuitionPartnerCoverage
                    {
                        TuitionType = db.TuitionTypes.First(),
                        LocalAuthorityDistrict = db.LocalAuthorityDistricts.First(),
                        PrimaryLiteracy = true,
                    }
                }
            });

            await db.SaveChangesAsync();
        });

        var result = await fixture.SendAsync(new Results.Query { Postcode = "-", Subjects = new[] { "x" } });

        result.Results.Results.Should().BeEquivalentTo(new[]
        {
            new { Name = "Alpha" },
        });
    }

    private async Task ResetDatabase()
    {
        await fixture.ExecuteDbContextAsync(db =>
        {
            db.LocalAuthorityDistricts.RemoveRange(db.LocalAuthorityDistricts);
            db.TuitionTypes.RemoveRange(db.TuitionTypes);
            db.TuitionPartners.RemoveRange(db.TuitionPartners);
            db.Regions.RemoveRange(db.Regions);
            db.TutorTypes.RemoveRange(db.TutorTypes);
            return db.SaveChangesAsync();
        });
    }
}