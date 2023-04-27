using Domain.Constants;
using Domain.Search;
using Tests.TestData;
using OrderByDirection = Domain.Enums.OrderByDirection;
using TuitionPartnerOrderBy = Domain.Enums.TuitionPartnerOrderBy;
using TuitionType = Domain.Enums.TuitionType;

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
            .WithName("quebec-tuition-partner", "Quebec")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionType.InSchool)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3ModernForeignLanguages, s => s
                    .InSchool().Costing(8m).ForGroupSizes(3)))
            );

        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithId(1)
            .WithName("oscar-tuition-partner", "Oscar")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionType.InSchool)
            .TaughtIn(District.NorthEastLincolnshire, TuitionType.InSchool, TuitionType.Online)
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
            .WithName("papa-tuition-partner", "Papa")
            .TaughtIn(District.NorthTyneside, TuitionType.InSchool)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3ModernForeignLanguages, s => s
                    .InSchool().Costing(14m).ForGroupSizes(4))
                .Subject(Subjects.Id.KeyStage3ModernForeignLanguages, s => s
                    .InSchool().Costing(13m).ForGroupSizes(5))
                )
            );

        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithId(4)
            .WithName("romeo-tuition-partner", "Romeo")
            .TaughtIn(District.Dacorum, TuitionType.InSchool, TuitionType.Online)
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
        { Name = "OSC" };
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
                { "quebec-tuition-partner" }
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
                { "papa-tuition-partner", "oscar-tuition-partner" }
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
            TuitionTypeId = (int)TuitionType.Online
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
            TuitionTypeId = (int)TuitionType.Online
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
            TuitionTypeId = (int)TuitionType.InSchool,
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
            TuitionTypeId = (int)TuitionType.InSchool,
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

        var oscar = results.First(x => x.Name == "Oscar");
        oscar.SubjectsCoverage.Should().NotBeEmpty();
        oscar.SubjectsCoverage!.Length.Should().Be(2);
        oscar.Prices.Should().NotBeEmpty();
        oscar.Prices!.Length.Should().Be(5);
        var prices = oscar.Prices.Select(x => new { x.GroupSize, x.HourlyRate }).ToList();
        prices.Should().BeEquivalentTo(new[]
        {
            new { GroupSize = 3, HourlyRate = 13m },
            new { GroupSize = 4, HourlyRate = 11m },
            new { GroupSize = 1, HourlyRate = 20m },
            new { GroupSize = 2, HourlyRate = 18m },
            new { GroupSize = 3, HourlyRate = 16m }
        });
        oscar.TuitionTypes.Should().NotBeEmpty();
        oscar.TuitionTypes!.Length.Should().Be(2);


        var papa = results.First(x => x.Name == "Papa");
        papa.SubjectsCoverage.Should().NotBeEmpty();
        papa.Prices.Should().NotBeEmpty();
        papa.TuitionTypes.Should().NotBeEmpty();
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

        //has TT, subjects and prices for Oscar TP for EastRidingOfYorkshire
        var oscar = results.First(x => x.Name == "Oscar");
        oscar.SubjectsCoverage.Should().NotBeEmpty();
        oscar.SubjectsCoverage!.Length.Should().Be(1);
        oscar.SubjectsCoverage[0].TuitionTypeId.Should().Be((int)TuitionType.InSchool);
        oscar.SubjectsCoverage[0].Subject.Id.Should().Be(Subjects.Id.KeyStage1English);
        oscar.Prices.Should().NotBeEmpty();
        oscar.Prices!.Length.Should().Be(2);
        var prices = oscar.Prices.Select(x => new { x.GroupSize, x.HourlyRate }).ToList();
        prices.Should().BeEquivalentTo(new[]
        {
            new { GroupSize = 3, HourlyRate = 13m },
            new { GroupSize = 4, HourlyRate = 11m }
        });
        oscar.TuitionTypes.Should().NotBeEmpty();
        oscar.TuitionTypes!.Length.Should().Be(1);
        oscar.TuitionTypes[0].Id.Should().Be((int)TuitionType.InSchool);


        //no TT, subjects or prices for Papa TP for EastRidingOfYorkshire
        var papa = results.First(x => x.Name == "Papa");
        papa.SubjectsCoverage.Should().BeEmpty();
        papa.Prices.Should().BeEmpty();
        papa.TuitionTypes.Should().BeEmpty();
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

        //has TT, subjects and prices for Papa TP for NorthTyneside
        var papa = results.First(x => x.Name == "Papa");
        papa.SubjectsCoverage.Should().NotBeEmpty();
        papa.SubjectsCoverage!.Length.Should().Be(1);
        papa.SubjectsCoverage[0].TuitionTypeId.Should().Be((int)TuitionType.InSchool);
        papa.SubjectsCoverage[0].Subject.Id.Should().Be(Subjects.Id.KeyStage3ModernForeignLanguages);
        papa.Prices.Should().NotBeEmpty();
        papa.Prices!.Length.Should().Be(2);
        var prices = papa.Prices.Select(x => new { x.GroupSize, x.HourlyRate }).ToList();
        prices.Should().BeEquivalentTo(new[]
        {
            new { GroupSize = 4, HourlyRate = 14m },
            new { GroupSize = 5, HourlyRate = 13m }
        });
        papa.TuitionTypes.Should().NotBeEmpty();
        papa.TuitionTypes!.Length.Should().Be(1);
        papa.TuitionTypes[0].Id.Should().Be((int)TuitionType.InSchool);


        //no TT, subjects or prices for Quebec TP for NorthTyneside
        var quebec = results.First(x => x.Name == "Quebec");
        quebec.SubjectsCoverage.Should().BeEmpty();
        quebec.Prices.Should().BeEmpty();
        quebec.TuitionTypes.Should().BeEmpty();
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
            TuitionTypeId = (int)TuitionType.InSchool,
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
        var oscar = refinedResults.First(x => x.Name == "Oscar");
        oscar!.Prices!.Length.Should().Be(1);
        oscar!.Prices[0].HourlyRate.Should().Be(11m);
        oscar!.TuitionTypes!.Length.Should().Be(1);
        oscar!.TuitionTypes[0].Id.Should().Be((int)TuitionType.InSchool);
        oscar!.SubjectsCoverage!.Length.Should().Be(1);
        oscar!.SubjectsCoverage[0].SubjectId.Should().Be(Subjects.Id.KeyStage1English);
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
            TuitionTypeId = (int)TuitionType.InSchool,
            SubjectIds = null
        };

        var refinedResults = Fixture.TuitionPartnerService.FilterTuitionPartnersData(results, dataFilter);

        refinedResults.Should().NotBeEmpty();
        var romeo = refinedResults.First(x => x.Name == "Romeo");
        romeo!.Prices!.Length.Should().Be(2);
        romeo!.Prices[0].HourlyRate.Should().Be(113m);
        romeo!.TuitionTypes!.Length.Should().Be(1);
        romeo!.TuitionTypes[0].Id.Should().Be((int)TuitionType.InSchool);
        romeo!.SubjectsCoverage!.Length.Should().Be(1);
        romeo!.SubjectsCoverage[0].SubjectId.Should().Be(Subjects.Id.KeyStage1English);
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
        var oscar = refinedResults.First(x => x.Name == "Oscar");
        oscar!.Prices!.Length.Should().Be(3);
        oscar!.Prices[0].HourlyRate.Should().Be(20m);
        oscar!.TuitionTypes!.Length.Should().Be(1);
        oscar!.TuitionTypes[0].Id.Should().Be((int)TuitionType.Online);
        oscar!.SubjectsCoverage!.Length.Should().Be(1);
        oscar!.SubjectsCoverage[0].SubjectId.Should().Be(Subjects.Id.KeyStage2Maths);
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
            new []{ "Oscar", "Papa", "Quebec", "Romeo" }
        };

        yield return new object[]
        {
            new TuitionPartnerOrdering { OrderBy = TuitionPartnerOrderBy.Name },
            new []{ "Oscar", "Papa", "Quebec", "Romeo" }
        };

        yield return new object[]
        {
            new TuitionPartnerOrdering { OrderBy = TuitionPartnerOrderBy.Name, Direction = OrderByDirection.Descending },
            new []{ "Romeo", "Quebec", "Papa", "Oscar"}
        };

        yield return new object[]
        {
            new TuitionPartnerOrdering { OrderBy = TuitionPartnerOrderBy.SeoList, SeoUrlOrderBy = new string[]{ "quebec-tuition-partner", "oscar-tuition-partner", "romeo-tuition-partner", "papa-tuition-partner" } },
            new []{ "Quebec", "Oscar", "Romeo", "Papa" }
        };

        yield return new object[]
        {
            new TuitionPartnerOrdering { OrderBy = TuitionPartnerOrderBy.Price },
            new []{ "Quebec", "Oscar", "Papa", "Romeo" }
        };

        yield return new object[]
        {
            new TuitionPartnerOrdering { OrderBy = TuitionPartnerOrderBy.Price, Direction = OrderByDirection.Descending },
            new []{ "Romeo", "Oscar", "Papa", "Quebec" }
        };
    }
    #endregion
}