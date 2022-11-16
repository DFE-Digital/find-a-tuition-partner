using Application.Handlers;
using Domain;
using Domain.Search;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class RandomiseSearchResults : IClassFixture<RandomiseSearchResultsFixture>
{
    private readonly SliceFixture _fixture;

    public RandomiseSearchResults(RandomiseSearchResultsFixture fixture)
        => _fixture = fixture.Fixture;

    [Fact]
    public void LAD_randomness()
    {
        var search = new TuitionPartnerSearchRequest { LocalAuthorityDistrictCode = "bob" };
        new TuitionPartnerOrdering(search).RandomSeed().Should().Be('b' + 'o' + 'b');
    }

    [Fact]
    public void Postcode_randomness()
    {
        var search = new TuitionPartnerSearchRequest { Postcode = "ts1 10n" };
        new TuitionPartnerOrdering(search).RandomSeed().Should().Be('t' + 's' + '1' + ' ' + '1' + '0' + 'n');
    }

    [Theory]
    [InlineData(0, 0, 1, 1)]
    [InlineData(1, 2, 3, 6)]
    [InlineData(8, 12, 23, 43)]
    public void Subject_randomness(int a, int b, int c, int total)
    {
        var search = new TuitionPartnerSearchRequest { SubjectIds = new[] { a, b, c } };
        new TuitionPartnerOrdering(search).RandomSeed().Should().Be(total);
    }

    [Fact]
    public void TuitionType_randomness()
    {
        var search = new TuitionPartnerSearchRequest { TuitionTypeId = 5 };
        new TuitionPartnerOrdering(search).RandomSeed().Should().Be(5);
    }

    [Fact]
    public void All_randomness()
    {
        var search = new TuitionPartnerSearchRequest
        {
            LocalAuthorityDistrictCode = "ab12",
            Postcode = "ts1 10n",
            SubjectIds = new[] { 5, 9, 22, 65 },
            TuitionTypeId = 5,
        };
        new TuitionPartnerOrdering(search).RandomSeed()
            .Should().Be('a' + 'b' + '1' + '2' + 't' + 's' + '1' + ' ' + '1' + '0' + 'n' + 5 + 9 + 22 + 65 + 5);
    }

    [Fact]
    public async void Search_results_can_be_randomised2()
    {
        var results = await _fixture.SendAsync(new SearchTuitionPartnerHandler.Command
        {
            OrderBy = TuitionPartnerOrderBy.Random,
        });

        results.Results.Should().NotBeEmpty();
        results.Results.Select(x => x.Name).Should()
            .ContainInOrder("Alpha", "Delta", "Bravo", "Charlie");
    }

    [Theory]
    [MemberData(nameof(SearchData))]
    public async void Search_results_can_be_randomised(
        SearchTuitionPartnerHandler.Command search, string[] order)
    {
        search.OrderBy = TuitionPartnerOrderBy.Random;

        var results = await _fixture.SendAsync(search);

        results.Results.Should().NotBeEmpty();
        results.Results.Select(x => x.Name)
            .Should().ContainInOrder(order)
            .And.Equal(order);
    }

    public static IEnumerable<object[]> SearchData()
    {
        yield return new object[]
        {
            new SearchTuitionPartnerHandler.Command { },
            new []{ "Alpha", "Delta", "Bravo", "Charlie", }
        };

        yield return new object[]
        {
            new SearchTuitionPartnerHandler.Command { LocalAuthorityDistrictCode = "E06000030" },
            new []{ "Delta", "Alpha", "Charlie", "Bravo",  }
        };

        yield return new object[]
        {
            new SearchTuitionPartnerHandler.Command { LocalAuthorityDistrictCode = "E07000179" },
            new []{  "Bravo", "Alpha", "Charlie", "Delta",  }
        };

        yield return new object[]
        {
            new SearchTuitionPartnerHandler.Command
            {
                LocalAuthorityDistrictCode = "E07000179",
                SubjectIds = new[] { 1, 2, 3, 4 }
            },
            new []{ "Delta", "Bravo", "Alpha", "Charlie", }
        };

        // Subject ID order doesn't matter
        yield return new object[]
        {
            new SearchTuitionPartnerHandler.Command
            {
                LocalAuthorityDistrictCode = "E07000179",
                SubjectIds = new[] { 4, 3, 2, 1 }
            },
            new []{ "Delta", "Bravo", "Alpha", "Charlie", }
        };

        // Subject ID values do matter
        yield return new object[]
        {
            new SearchTuitionPartnerHandler.Command
            {
                LocalAuthorityDistrictCode = "E07000179",
                SubjectIds = new[] { 4, 5, 6, 7, 8, 9 }
            },
            new []{ "Charlie", "Delta", "Bravo", "Alpha", }
        };

        yield return new object[]
        {
            new SearchTuitionPartnerHandler.Command
            {
                LocalAuthorityDistrictCode = "E06000057",
                SubjectIds = new[] { 4, 3, 2, 1 },
                TuitionTypeId = 1,
            },
            new []{ "Charlie", "Alpha", "Delta", "Bravo", }
        };

        yield return new object[]
        {
            new SearchTuitionPartnerHandler.Command
            {
                LocalAuthorityDistrictCode = "E06000057",
                SubjectIds = new[] { 4, 3, 2, 1 },
                TuitionTypeId = 2,
            },
            new []{ "Delta", "Bravo", "Charlie", "Alpha", }
        };
    }
}

public class RandomiseSearchResultsFixture : IAsyncLifetime
{
    public RandomiseSearchResultsFixture(SliceFixture fixture) => Fixture = fixture;

    public SliceFixture Fixture { get; }

    public async Task InitializeAsync()
    {
        await Fixture.ExecuteDbContextAsync(async db =>
        {
            db.Prices.RemoveRange(db.Prices);
            db.SubjectCoverage.RemoveRange(db.SubjectCoverage);
            db.TuitionPartners.RemoveRange(db.TuitionPartners);
            await db.SaveChangesAsync();

            db.TuitionPartners.Add(CreateTuitionPartner("Alpha"));
            db.TuitionPartners.Add(CreateTuitionPartner("Bravo"));
            db.TuitionPartners.Add(CreateTuitionPartner("Charlie"));
            db.TuitionPartners.Add(CreateTuitionPartner("Delta"));

            await db.SaveChangesAsync();

            TuitionPartner CreateTuitionPartner(string name) => new TuitionPartner
            {
                SeoUrl = $"{name.ToLower()}-tuition-partner",
                Name = name,
                Website = $"https://tuition-partner.testdata/{name}",
                Description = $"{name} Description",
                LocalAuthorityDistrictCoverage = CreateAreaCoverage(),
                SubjectCoverage = CreateSubjectCoverage(),
            };

            List<LocalAuthorityDistrictCoverage> CreateAreaCoverage() =>
            (from ladc in db.LocalAuthorityDistricts
             from tt in db.TuitionTypes
             select new LocalAuthorityDistrictCoverage
             {
                 LocalAuthorityDistrictId = ladc.Id,
                 TuitionTypeId = tt.Id,
             })
             .ToList();

            List<SubjectCoverage> CreateSubjectCoverage() =>
                (from tt in db.TuitionTypes
                 from s in db.Subjects
                 select new SubjectCoverage { TuitionTypeId = tt.Id, SubjectId = s.Id }
                 ).ToList();
        });
    }

    public Task DisposeAsync() => Task.CompletedTask;
}