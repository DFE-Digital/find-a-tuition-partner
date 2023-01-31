using Domain;
using Domain.Enums;
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
        var seed = TuitionPartnerOrdering.RandomSeedGeneration(localAuthorityDistrictCode: "bob");
        seed.Should().Be('b' + 'o' + 'b');
    }

    [Fact]
    public void Postcode_randomness()
    {
        var seed = TuitionPartnerOrdering.RandomSeedGeneration(postcode: "ts1 10n");
        seed.Should().Be('t' + 's' + '1' + ' ' + '1' + '0' + 'n');
    }

    [Theory]
    [InlineData(0, 0, 1, 1)]
    [InlineData(1, 2, 3, 6)]
    [InlineData(8, 12, 23, 43)]
    public void Subject_randomness(int a, int b, int c, int total)
    {
        var seed = TuitionPartnerOrdering.RandomSeedGeneration(subjectIds: new[] { a, b, c });
        seed.Should().Be(total);
    }

    [Fact]
    public void TuitionType_randomness()
    {
        var seed = TuitionPartnerOrdering.RandomSeedGeneration(tuitionFilterId: 5);
        seed.Should().Be(5);
    }

    [Fact]
    public void All_randomness()
    {
        var seed = TuitionPartnerOrdering.RandomSeedGeneration(localAuthorityDistrictCode: "ab12", postcode: "ts1 10n", subjectIds: new[] { 5, 9, 22, 65 }, tuitionFilterId: 5);
        seed.Should().Be('a' + 'b' + '1' + '2' + 't' + 's' + '1' + ' ' + '1' + '0' + 'n' + 5 + 9 + 22 + 65 + 5);
    }

    [Fact]
    public async void Search_results_can_be_randomised2()
    {
        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var results = await _fixture.TuitionPartnerService.GetTuitionPartnersAsync(new TuitionPartnerRequest(), cancellationToken);
        results = _fixture.TuitionPartnerService.OrderTuitionPartners(results, new TuitionPartnerOrdering
        {
            OrderBy = TuitionPartnerOrderBy.Random,
            RandomSeed = TuitionPartnerOrdering.RandomSeedGeneration()
        });

        results.Should().NotBeEmpty();
        results.Select(x => x.Name).Should()
            .ContainInOrder("Delta", "Alpha", "Charlie", "Bravo");
    }

    [Theory]
    [MemberData(nameof(SearchData))]
    public async void Search_results_can_be_randomised(
        TuitionPartnersFilter filter, string[] order, string? localAuthorityDistrictCode = null, string? postcode = null)
    {
        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var tuitionPartnersIds = await _fixture.TuitionPartnerService.GetTuitionPartnersFilteredAsync(filter, cancellationToken);

        var tuitionPartners = await _fixture.TuitionPartnerService.GetTuitionPartnersAsync(new TuitionPartnerRequest
        {
            TuitionPartnerIds = tuitionPartnersIds,
            LocalAuthorityDistrictId = filter.LocalAuthorityDistrictId
        }, cancellationToken);

        var results = _fixture.TuitionPartnerService.OrderTuitionPartners(tuitionPartners, new TuitionPartnerOrdering
        {
            OrderBy = TuitionPartnerOrderBy.Random,
            RandomSeed = TuitionPartnerOrdering.RandomSeedGeneration(localAuthorityDistrictCode, postcode, filter.SubjectIds, filter.TuitionTypeId)
        });

        results.Should().NotBeEmpty();
        results.Select(x => x.Name)
            .Should().ContainInOrder(order)
            .And.Equal(order);
    }

    public static IEnumerable<object[]> SearchData()
    {
        yield return new object[]
        {
            new TuitionPartnersFilter { },
            new []{ "Delta", "Alpha", "Charlie", "Bravo", }
        };

        yield return new object[]
        {
            new TuitionPartnersFilter { LocalAuthorityDistrictId = 58 }, //58 = E06000030
            new []{ "Alpha", "Delta", "Bravo", "Charlie",  },
            "E06000030"
        };

        yield return new object[]
        {
            new TuitionPartnersFilter { LocalAuthorityDistrictId = 144 }, //144 = E07000179
            new []{ "Charlie", "Delta", "Bravo", "Alpha",  },
            "E07000179"
        };

        yield return new object[]
        {
            new TuitionPartnersFilter
            {
                LocalAuthorityDistrictId = 144, //144 = E07000179
                SubjectIds = new[] { 1, 2, 3, 4 }
            },
            new []{ "Alpha", "Charlie", "Delta", "Bravo", },
            "E07000179"
        };

        // Subject ID order doesn't matter
        yield return new object[]
        {
            new TuitionPartnersFilter
            {
                LocalAuthorityDistrictId = 144, //144 = E07000179
                SubjectIds = new[] { 4, 3, 2, 1 }
            },
            new []{ "Alpha", "Charlie", "Delta", "Bravo", },
            "E07000179"
        };

        // Subject ID values do matter
        yield return new object[]
        {
            new TuitionPartnersFilter
            {
                LocalAuthorityDistrictId = 144, //144 = E07000179
                SubjectIds = new[] { 4, 5, 6, 7, 8, 9 }
            },
            new []{ "Bravo", "Alpha", "Charlie", "Delta", },
            "E07000179"
        };

        yield return new object[]
        {
            new TuitionPartnersFilter
            {
                LocalAuthorityDistrictId = 191, //191 = E06000057
                SubjectIds = new[] { 4, 3, 2, 1 },
                TuitionTypeId = 1,
            },
            new []{ "Bravo", "Delta", "Alpha", "Charlie", },
            "E06000057"
        };

        yield return new object[]
        {
            new TuitionPartnersFilter
            {
                LocalAuthorityDistrictId = 191, //191 = E06000057
                SubjectIds = new[] { 4, 3, 2, 1 },
                TuitionTypeId = 2,
            },
            new []{ "Alpha", "Charlie", "Bravo", "Delta", },
            "E06000057"
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

            TuitionPartner CreateTuitionPartner(string name) => new()
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