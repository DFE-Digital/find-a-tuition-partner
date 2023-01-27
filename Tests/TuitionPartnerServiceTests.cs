using Domain.Constants;
using Domain.Search;
using Tests.TestData;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class TuitionPartnerServiceTests : CleanSliceFixture
{
    public TuitionPartnerServiceTests(SliceFixture fixture) : base(fixture)
    {
    }

    private async void SetUpGetTuitionPartnersFilteredData()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithId(3)
            .WithName("charlie-tuition-partner", "Charlie")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionTypes.InSchool)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3ModernForeignLanguages, s => s
                    .InSchool().Costing(8m).ForGroupSizes(3)))
            );

        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithId(1)
            .WithName("alpha-tuition-partner", "Alpha")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionTypes.InSchool)
            .TaughtIn(District.NorthEastLincolnshire, TuitionTypes.InSchool, TuitionTypes.Online)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage1English, s => s
                    .InSchool().Costing(13m).ForGroupSizes(3))
                .Subject(Subjects.Id.KeyStage1English, s => s
                    .InSchool().Costing(11m).ForGroupSizes(4))
                .Subject(Subjects.Id.KeyStage2Maths, s => s
                    .Online().Costing(20m).ForGroupSizes(1))
                .Subject(Subjects.Id.KeyStage2Maths, s => s
                    .Online().Costing(18m).ForGroupSizes(2))
                .Subject(Subjects.Id.KeyStage2Maths, s => s
                    .Online().Costing(16m).ForGroupSizes(3))
                )
            );


        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithId(2)
            .WithName("bravo-tuition-partner", "Bravo")
            .TaughtIn(District.NorthTyneside, TuitionTypes.InSchool)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3ModernForeignLanguages, s => s
                    .InSchool().Costing(14m).ForGroupSizes(4))
                .Subject(Subjects.Id.KeyStage3ModernForeignLanguages, s => s
                    .InSchool().Costing(13m).ForGroupSizes(5))
                )
            );

        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithId(4)
            .WithName("delta-tuition-partner", "Delta")
            .TaughtIn(District.Dacorum, TuitionTypes.InSchool, TuitionTypes.Online)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage1English, s => s
                    .InSchool().Costing(113m).ForGroupSizes(3))
                .Subject(Subjects.Id.KeyStage1English, s => s
                    .InSchool().Costing(111m).ForGroupSizes(4))
                .Subject(Subjects.Id.KeyStage2Maths, s => s
                    .Online().Costing(120m).ForGroupSizes(1))
                .Subject(Subjects.Id.KeyStage2Maths, s => s
                    .Online().Costing(118m).ForGroupSizes(2))
                .Subject(Subjects.Id.KeyStage2Maths, s => s
                    .Online().Costing(116m).ForGroupSizes(3))
                )
            );
    }

    #region GetTuitionPartnersFiltered
    [Fact]
    public async void GetTuitionPartnersFiltered_all()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var filter = new TuitionPartnersFilter();
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersFilteredAsync(filter, cancellationToken);

        results.Should().NotBeEmpty();
        results?.Length.Should().Be(4);
    }

    [Fact]
    public async void GetTuitionPartnersFiltered_name_match()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var filter = new TuitionPartnersFilter()
        { Name = "ALP" };
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersFilteredAsync(filter, cancellationToken);

        results.Should().NotBeEmpty();
        results?.Length.Should().Be(1);
    }

    [Fact]
    public async void GetTuitionPartnersFiltered_name_no_match()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var filter = new TuitionPartnersFilter()
        { Name = "ZZZ" };
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersFilteredAsync(filter, cancellationToken);

        results.Should().BeEmpty();
    }

    [Fact]
    public async void GetTuitionPartnersFiltered_seo_single_match()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var filter = new TuitionPartnersFilter()
        {
            SeoUrls = new string[]
                { "charlie-tuition-partner" }
        };
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersFilteredAsync(filter, cancellationToken);

        results.Should().NotBeEmpty();
        results?.Length.Should().Be(1);
    }

    [Fact]
    public async void GetTuitionPartnersFiltered_seo_multiple_match()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var filter = new TuitionPartnersFilter()
        {
            SeoUrls = new string[]
                { "bravo-tuition-partner", "alpha-tuition-partner" }
        };
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersFilteredAsync(filter, cancellationToken);

        results.Should().NotBeEmpty();
        results?.Length.Should().Be(2);
    }

    [Fact]
    public async void GetTuitionPartnersFiltered_seo_no_match()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var filter = new TuitionPartnersFilter()
        {
            SeoUrls = new string[]
                { "jhjhjh" }
        };
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersFilteredAsync(filter, cancellationToken);

        results.Should().BeEmpty();
    }

    [Fact]
    public async void GetTuitionPartnersFiltered_lad_no_match()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var filter = new TuitionPartnersFilter()
        {
            LocalAuthorityDistrictId = District.Calderdale.Id
        };
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersFilteredAsync(filter, cancellationToken);

        results.Should().BeEmpty();
    }

    [Fact]
    public async void GetTuitionPartnersFiltered_lad_single_match()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var filter = new TuitionPartnersFilter()
        {
            LocalAuthorityDistrictId = District.NorthTyneside.Id
        };
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersFilteredAsync(filter, cancellationToken);

        results.Should().NotBeEmpty();
        results?.Length.Should().Be(1);
    }

    [Fact]
    public async void GetTuitionPartnersFiltered_lad_multiple_match()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var filter = new TuitionPartnersFilter()
        {
            LocalAuthorityDistrictId = District.EastRidingOfYorkshire.Id
        };
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersFilteredAsync(filter, cancellationToken);

        results.Should().NotBeEmpty();
        results?.Length.Should().Be(2);
    }

    [Fact]
    public async void GetTuitionPartnersFiltered_lad_plus_tuition_type_no_match()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var filter = new TuitionPartnersFilter()
        {
            LocalAuthorityDistrictId = District.NorthTyneside.Id,
            TuitionTypeId = (int)TuitionTypes.Online
        };
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersFilteredAsync(filter, cancellationToken);

        results.Should().BeEmpty();
    }

    [Fact]
    public async void GetTuitionPartnersFiltered_lad_plus_tuition_type_single_match()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var filter = new TuitionPartnersFilter()
        {
            LocalAuthorityDistrictId = District.NorthEastLincolnshire.Id,
            TuitionTypeId = (int)TuitionTypes.Online
        };
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersFilteredAsync(filter, cancellationToken);

        results.Should().NotBeEmpty();
        results?.Length.Should().Be(1);
    }

    [Fact]
    public async void GetTuitionPartnersFiltered_lad_plus_tuition_type_plus_subject_no_match()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var filter = new TuitionPartnersFilter()
        {
            LocalAuthorityDistrictId = District.EastRidingOfYorkshire.Id,
            TuitionTypeId = (int)TuitionTypes.InSchool,
            SubjectIds = new List<int>() { Subjects.Id.KeyStage2Maths }
        };
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersFilteredAsync(filter, cancellationToken);

        results.Should().BeEmpty();
    }

    [Fact]
    public async void GetTuitionPartnersFiltered_lad_plus_tuition_type_plus_subject_single_match()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var filter = new TuitionPartnersFilter()
        {
            LocalAuthorityDistrictId = District.EastRidingOfYorkshire.Id,
            TuitionTypeId = (int)TuitionTypes.InSchool,
            SubjectIds = new List<int>() { Subjects.Id.KeyStage1English }
        };
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersFilteredAsync(filter, cancellationToken);

        results.Should().NotBeEmpty();
        results?.Length.Should().Be(1);
    }
    #endregion

    #region GetTuitionPartnersAsync
    [Fact]
    public async void GetTuitionPartnersAsync_no_match()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var request = new TuitionPartnerRequest()
        {
            TuitionPartnerIds = Array.Empty<int>(),
            LocalAuthorityDistrictId = District.EastRidingOfYorkshire.Id
        };
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersAsync(request, cancellationToken);

        results.Should().BeEmpty();
    }

    [Fact]
    public async void GetTuitionPartnersAsync_all()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var request = new TuitionPartnerRequest();
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersAsync(request, cancellationToken);

        results.Should().NotBeEmpty();
        results.Count().Should().Be(4);

        var alpha = results.First(x => x.Name == "Alpha");
        alpha.SubjectsCoverage.Should().NotBeEmpty();
        alpha.SubjectsCoverage!.Length.Should().Be(2);
        alpha.Prices.Should().NotBeEmpty();
        alpha.Prices!.Length.Should().Be(5);
        var prices = alpha.Prices.Select(x => new { x.GroupSize, x.HourlyRate }).ToList();
        prices.Should().BeEquivalentTo(new[]
        {
            new { GroupSize = 3, HourlyRate = 13m },
            new { GroupSize = 4, HourlyRate = 11m },
            new { GroupSize = 1, HourlyRate = 20m },
            new { GroupSize = 2, HourlyRate = 18m },
            new { GroupSize = 3, HourlyRate = 16m }
        });
        alpha.TuitionTypes.Should().NotBeEmpty();
        alpha.TuitionTypes!.Length.Should().Be(2);


        var bravo = results.First(x => x.Name == "Bravo");
        bravo.SubjectsCoverage.Should().NotBeEmpty();
        bravo.Prices.Should().NotBeEmpty();
        bravo.TuitionTypes.Should().NotBeEmpty();
    }

    [Fact]
    public async void GetTuitionPartnersAsync_all_lad_filter()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var request = new TuitionPartnerRequest()
        {
            TuitionPartnerIds = null,
            LocalAuthorityDistrictId = District.EastRidingOfYorkshire.Id
        };
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersAsync(request, cancellationToken);

        results.Should().NotBeEmpty();
        results.Count().Should().Be(4);

        //has TT, subjects and prices for Alpha TP for EastRidingOfYorkshire
        var alpha = results.First(x => x.Name == "Alpha");
        alpha.SubjectsCoverage.Should().NotBeEmpty();
        alpha.SubjectsCoverage!.Length.Should().Be(1);
        alpha.SubjectsCoverage[0].TuitionTypeId.Should().Be((int)TuitionTypes.InSchool);
        alpha.SubjectsCoverage[0].Subject.Id.Should().Be(Subjects.Id.KeyStage1English);
        alpha.Prices.Should().NotBeEmpty();
        alpha.Prices!.Length.Should().Be(2);
        var prices = alpha.Prices.Select(x => new { x.GroupSize, x.HourlyRate }).ToList();
        prices.Should().BeEquivalentTo(new[]
        {
            new { GroupSize = 3, HourlyRate = 13m },
            new { GroupSize = 4, HourlyRate = 11m }
        });
        alpha.TuitionTypes.Should().NotBeEmpty();
        alpha.TuitionTypes!.Length.Should().Be(1);
        alpha.TuitionTypes[0].Id.Should().Be((int)TuitionTypes.InSchool);


        //no TT, subjects or prices for Bravo TP for EastRidingOfYorkshire
        var bravo = results.First(x => x.Name == "Bravo");
        bravo.SubjectsCoverage.Should().BeEmpty();
        bravo.Prices.Should().BeEmpty();
        bravo.TuitionTypes.Should().BeEmpty();
    }

    [Fact]
    public async void GetTuitionPartnersAsync_by_ids_and_lad()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var request = new TuitionPartnerRequest()
        {
            TuitionPartnerIds = new int[] { 2, 3 },
            LocalAuthorityDistrictId = District.NorthTyneside.Id
        };
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersAsync(request, cancellationToken);

        results.Should().NotBeEmpty();
        results.Count().Should().Be(2);

        //has TT, subjects and prices for Bravo TP for NorthTyneside
        var bravo = results.First(x => x.Name == "Bravo");
        bravo.SubjectsCoverage.Should().NotBeEmpty();
        bravo.SubjectsCoverage!.Length.Should().Be(1);
        bravo.SubjectsCoverage[0].TuitionTypeId.Should().Be((int)TuitionTypes.InSchool);
        bravo.SubjectsCoverage[0].Subject.Id.Should().Be(Subjects.Id.KeyStage3ModernForeignLanguages);
        bravo.Prices.Should().NotBeEmpty();
        bravo.Prices!.Length.Should().Be(2);
        var prices = bravo.Prices.Select(x => new { x.GroupSize, x.HourlyRate }).ToList();
        prices.Should().BeEquivalentTo(new[]
        {
            new { GroupSize = 4, HourlyRate = 14m },
            new { GroupSize = 5, HourlyRate = 13m }
        });
        bravo.TuitionTypes.Should().NotBeEmpty();
        bravo.TuitionTypes!.Length.Should().Be(1);
        bravo.TuitionTypes[0].Id.Should().Be((int)TuitionTypes.InSchool);


        //no TT, subjects or prices for Charlie TP for NorthTyneside
        var charlie = results.First(x => x.Name == "Charlie");
        charlie.SubjectsCoverage.Should().BeEmpty();
        charlie.Prices.Should().BeEmpty();
        charlie.TuitionTypes.Should().BeEmpty();
    }
    #endregion

    #region FilterTuitionPartnersData
    [Fact]
    public async void FilterTuitionPartnersData_no_data()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var request = new TuitionPartnerRequest();
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersAsync(request, cancellationToken);
        results.Count().Should().Be(4);

        var dataFilter = new TuitionPartnersDataFilter()
        {
            GroupSize = 6,
            TuitionTypeId = (int)Domain.Enums.TuitionType.InSchool,
            SubjectIds = new List<int>() { Subjects.Id.KeyStage4ModernForeignLanguages }
        };

        var refinedResults = Fixture.TuitionPartnerService.FilterTuitionPartnersData(results, dataFilter);

        refinedResults.Should().NotBeEmpty();
        foreach (var refinedResult in refinedResults)
        {
            refinedResult.Prices.Should().BeNull();
            refinedResult.TuitionTypes.Should().BeNull();
            refinedResult.SubjectsCoverage.Should().BeNull();

            refinedResult.Name.Should().NotBeEmpty();
        }
    }

    [Fact]
    public async void FilterTuitionPartnersData_all()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var request = new TuitionPartnerRequest();
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersAsync(request, cancellationToken);
        results.Count().Should().Be(4);

        var dataFilter = new TuitionPartnersDataFilter()
        {
            GroupSize = null,
            TuitionTypeId = null,
            SubjectIds = null
        };

        var refinedResults = Fixture.TuitionPartnerService.FilterTuitionPartnersData(results, dataFilter);

        refinedResults.Should().NotBeEmpty();
        foreach (var refinedResult in refinedResults)
        {
            refinedResult.Prices.Should().NotBeEmpty();
            refinedResult.TuitionTypes.Should().NotBeEmpty();
            refinedResult.SubjectsCoverage.Should().NotBeEmpty();

            refinedResult.Name.Should().NotBeEmpty();
        }
    }

    [Fact]
    public async void FilterTuitionPartnersData_by_group_size()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var request = new TuitionPartnerRequest();
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersAsync(request, cancellationToken);
        results.Count().Should().Be(4);

        var dataFilter = new TuitionPartnersDataFilter()
        {
            GroupSize = 4,
            TuitionTypeId = null,
            SubjectIds = null
        };

        var refinedResults = Fixture.TuitionPartnerService.FilterTuitionPartnersData(results, dataFilter);

        refinedResults.Should().NotBeEmpty();
        var alpha = refinedResults.First(x => x.Name == "Alpha");
        alpha!.Prices!.Length.Should().Be(1);
        alpha!.Prices[0].HourlyRate.Should().Be(11m);
        alpha!.TuitionTypes!.Length.Should().Be(1);
        alpha!.TuitionTypes[0].Id.Should().Be((int)Domain.Enums.TuitionType.InSchool);
        alpha!.SubjectsCoverage!.Length.Should().Be(1);
        alpha!.SubjectsCoverage[0].SubjectId.Should().Be(Subjects.Id.KeyStage1English);
    }

    [Fact]
    public async void FilterTuitionPartnersData_by_tuition_type()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var request = new TuitionPartnerRequest();
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersAsync(request, cancellationToken);
        results.Count().Should().Be(4);

        var dataFilter = new TuitionPartnersDataFilter()
        {
            GroupSize = null,
            TuitionTypeId = (int)Domain.Enums.TuitionType.InSchool,
            SubjectIds = null
        };

        var refinedResults = Fixture.TuitionPartnerService.FilterTuitionPartnersData(results, dataFilter);

        refinedResults.Should().NotBeEmpty();
        var delta = refinedResults.First(x => x.Name == "Delta");
        delta!.Prices!.Length.Should().Be(2);
        delta!.Prices[0].HourlyRate.Should().Be(113m);
        delta!.TuitionTypes!.Length.Should().Be(1);
        delta!.TuitionTypes[0].Id.Should().Be((int)Domain.Enums.TuitionType.InSchool);
        delta!.SubjectsCoverage!.Length.Should().Be(1);
        delta!.SubjectsCoverage[0].SubjectId.Should().Be(Subjects.Id.KeyStage1English);
    }

    [Fact]
    public async void FilterTuitionPartnersData_by_subjects()
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var request = new TuitionPartnerRequest();
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersAsync(request, cancellationToken);
        results.Count().Should().Be(4);

        var dataFilter = new TuitionPartnersDataFilter()
        {
            GroupSize = null,
            TuitionTypeId = null,
            SubjectIds = new List<int>() { Subjects.Id.KeyStage2Maths }
        };

        var refinedResults = Fixture.TuitionPartnerService.FilterTuitionPartnersData(results, dataFilter);

        refinedResults.Should().NotBeEmpty();
        var alpha = refinedResults.First(x => x.Name == "Alpha");
        alpha!.Prices!.Length.Should().Be(3);
        alpha!.Prices[0].HourlyRate.Should().Be(20m);
        alpha!.TuitionTypes!.Length.Should().Be(1);
        alpha!.TuitionTypes[0].Id.Should().Be((int)Domain.Enums.TuitionType.Online);
        alpha!.SubjectsCoverage!.Length.Should().Be(1);
        alpha!.SubjectsCoverage[0].SubjectId.Should().Be(Subjects.Id.KeyStage2Maths);
    }
    #endregion

    #region OrderTuitionPartners
    [Theory]
    [MemberData(nameof(OrderingTestData))]
    public async void OrderTuitionPartners(TuitionPartnerOrdering ordering, string[] order)
    {
        SetUpGetTuitionPartnersFilteredData();

        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;

        var request = new TuitionPartnerRequest();
        var results = await Fixture.TuitionPartnerService.GetTuitionPartnersAsync(request, cancellationToken);
        results.Should().NotBeEmpty();

        var orderedResults = Fixture.TuitionPartnerService.OrderTuitionPartners(results, ordering);

        orderedResults.Should().NotBeEmpty();
        orderedResults.Select(x => x.Name)
            .Should().ContainInOrder(order)
            .And.Equal(order);
    }

    public static IEnumerable<object[]> OrderingTestData()
    {
        yield return new object[]
        {
            new TuitionPartnerOrdering { },
            new []{ "Alpha", "Bravo", "Charlie", "Delta" }
        };

        yield return new object[]
        {
            new TuitionPartnerOrdering { OrderBy = Domain.Enums.TuitionPartnerOrderBy.Name },
            new []{ "Alpha", "Bravo", "Charlie", "Delta" }
        };

        yield return new object[]
        {
            new TuitionPartnerOrdering { OrderBy = Domain.Enums.TuitionPartnerOrderBy.Name, Direction = Domain.Enums.OrderByDirection.Descending },
            new []{ "Delta", "Charlie", "Bravo", "Alpha"}
        };

        yield return new object[]
        {
            new TuitionPartnerOrdering { OrderBy = Domain.Enums.TuitionPartnerOrderBy.SeoList, SeoUrlOrderBy = new string[]{ "charlie-tuition-partner", "alpha-tuition-partner", "delta-tuition-partner", "bravo-tuition-partner" } },
            new []{ "Charlie", "Alpha", "Delta", "Bravo" }
        };

        yield return new object[]
        {
            new TuitionPartnerOrdering { OrderBy = Domain.Enums.TuitionPartnerOrderBy.MinPrice },
            new []{ "Charlie", "Alpha", "Bravo", "Delta" }
        };

        yield return new object[]
        {
            new TuitionPartnerOrdering { OrderBy = Domain.Enums.TuitionPartnerOrderBy.MinPrice, Direction = Domain.Enums.OrderByDirection.Descending },
            new []{ "Delta", "Alpha", "Bravo", "Charlie" }
        };
    }
    #endregion
}