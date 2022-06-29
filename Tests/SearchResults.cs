using Domain;
using Domain.Search;
using NSubstitute;
using UI.Pages.FindATuitionPartner;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class SearchResults : CleanSliceFixture
{
    public SearchResults(SliceFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Displays_all_tutor_types_in_database2()
    {
        Fixture.LocationFilter.GetLocationFilterParametersAsync(Arg.Any<string>())
            .Returns(new LocationFilterParameters { LocalAuthorityDistrictCode = "N1" });

        await Fixture.ExecuteDbContextAsync(async db =>
        {
            db.Regions.AddRange(
                new Region { Code = "-", Name = "-", }
                );

            db.TuitionTypes.Add(new Domain.TuitionType { Name = "In Person" });

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

            db.TuitionPartners.Add(new Domain.TuitionPartner
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

        var result = await Fixture.SendAsync(new Results.Command
        {
            Postcode = "AB00BA",
            Subjects = new[] { "English" }
        });

        result.Results.Should().NotBeNull();
        result.Results!.Results.Should().BeEquivalentTo(new[]
        {
            new { Name = "Alpha" },
        });
    }
}