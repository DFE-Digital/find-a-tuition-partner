using Domain.Constants;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tests.TestData;
using UI.MediatR.Queries;
using UI.Structs;
using KeyStage = UI.Enums.KeyStage;
using TuitionPartner = UI.Pages.TuitionPartner;
using TuitionType = UI.Enums.TuitionType;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class TuitionPartnerDetailsPage : CleanSliceFixture
{
    public TuitionPartnerDetailsPage(SliceFixture fixture) : base(fixture)
    {
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Get_with_null_or_whitespace_id(string id)
    {
        var result = await Fixture.GetPage<TuitionPartner>()
            .Execute(page => page.OnGetAsync(new GetTuitionPartnerQuery(id)));
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData("A Tuition Partner")]
    [InlineData("A-Tuition-Partner")]
    public async Task Redirect_to_seo_url(string id)
    {
        var result = await Fixture.GetPage<TuitionPartner>()
            .Execute(page => page.OnGetAsync(new GetTuitionPartnerQuery(id)));
        result.Should().BeAssignableTo<RedirectToPageResult>()
            .Which.RouteValues.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                {"Id", "a-tuition-partner"}
            });
    }

    [Fact]
    public async Task Tuition_partner_not_found()
    {
        var result = await Fixture.GetPage<TuitionPartner>()
            .Execute(page => page.OnGetAsync(new GetTuitionPartnerQuery("not-found")));
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData("9")]
    [InlineData("this-tuition-partner")]
    public async Task Get_with_valid_id(string id)
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithId(9)
            .WithName("this-tuition-partner", "This Tuition Partner"));

        var result = await Fixture.GetPage<TuitionPartner>()
            .Execute(page => page.OnGetAsync(new GetTuitionPartnerQuery(id)));
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task Returns_null_when_not_found()
    {
        var result = await Fixture.SendAsync(new GetTuitionPartnerQuery("not-found"));
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("1")]
    [InlineData("a-tuition-partner")]
    public async Task Returns_tuition_partner_details(string id)
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithId(1)
            .WithName("a-tuition-partner", "A Tuition Partner")
            .WithDescription("A Tuition Partner Description")
            .WithWebsite("https://a-tuition-partner.testdata/ntp")
            .WithPhoneNumber("0123456789")
            .WithEmailAddress("ntp@a-tuition-partner.testdata")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionTypes.InSchool)
            .TaughtIn(District.NorthEastLincolnshire, TuitionTypes.InSchool, TuitionTypes.Online)
            .TaughtIn(District.Ryedale, TuitionTypes.Online)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3English, s => s
                    .InSchool().Costing(12.34m).ForGroupSizes(2, 3))
                .Subject(Subjects.Id.KeyStage3Maths, s => s
                    .InSchool().Costing(56.78m).ForGroupSizes(2, 3)
                    .Online().Costing(56.78m).ForGroupSizes(3)))
        );

        var result = await Fixture.SendAsync(new GetTuitionPartnerQuery(id));

        result!.Name.Should().Be("A Tuition Partner");
        result.Description.Should().Be("A Tuition Partner Description");
        result.Subjects.Should().BeEquivalentTo("Key stage 3 - English and Maths");
        result.Ratios.Should().BeEquivalentTo("1 to 2", "1 to 3");
        result.Prices.Should().BeEquivalentTo(new Dictionary<int, GroupPrice>
        {
            {2, new(12.34m, 56.78m, null, null)},
            {3, new(12.34m, 56.78m, 56.78m, 56.78m)}
        });
        result.Website.Should().Be("https://a-tuition-partner.testdata/ntp");
        result.PhoneNumber.Should().Be("0123456789");
        result.EmailAddress.Should().Be("ntp@a-tuition-partner.testdata");
    }

    [Fact]
    public async Task Shows_all_tuition_types_no_district_specified()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionTypes.InSchool, TuitionTypes.Online));

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery("a-tuition-partner"));

        result!.TuitionTypes.Should().BeEquivalentTo("Online", "In School");
    }

    [Fact]
    public async Task Shows_all_tuition_types_when_provided_in_district()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionTypes.InSchool, TuitionTypes.Online));

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery(
                    "a-tuition-partner")
            { Postcode = District.EastRidingOfYorkshire.SamplePostcode });

        result!.TuitionTypes.Should().BeEquivalentTo("Online", "In School");
    }

    [Fact]
    public async Task Shows_only_tuition_types_provided_in_district()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.Ryedale, TuitionTypes.Online));

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery(
                    "a-tuition-partner")
            { Postcode = District.Ryedale.SamplePostcode });

        result!.TuitionTypes.Should().BeEquivalentTo("Online");
    }

    [Fact]
    public async Task Shows_all_tuition_types_prices_when_provided_in_district()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.NorthEastLincolnshire, TuitionTypes.InSchool, TuitionTypes.Online)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3English, s => s
                    .InSchool().Costing(12.34m).ForGroupSizes(2, 3))
                .Subject(Subjects.Id.KeyStage3Maths, s => s
                    .InSchool().Costing(56.78m).ForGroupSizes(2, 3)
                    .Online().Costing(56.78m).ForGroupSizes(3))));

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery(
                    "a-tuition-partner")
            { Postcode = District.NorthEastLincolnshire.SamplePostcode });

        result!.Prices.Should().BeEquivalentTo(new Dictionary<int, GroupPrice>
        {
            {2, new(12.34m, 56.78m, null, null)},
            {3, new(12.34m, 56.78m, 56.78m, 56.78m)},
        });
    }

    [Fact]
    public async Task Shows_only_tuition_type_prices_provided_in_district()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.NorthEastLincolnshire, TuitionTypes.InSchool)
            .TaughtIn(District.Ryedale, TuitionTypes.Online)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3Maths, s => s
                    .InSchool().Costing(33.44m).ForGroupSizes(2)
                    .Online().Costing(56.78m).ForGroupSizes(3))));

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery(
                    "a-tuition-partner")
            { Postcode = District.Ryedale.SamplePostcode });

        result!.Prices.Should().BeEquivalentTo(new Dictionary<int, GroupPrice>
        {
            {3, new(null, null, 56.78m, 56.78m)}
        });
    }

    [Fact]
    public async Task Shows_all_locations()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionTypes.InSchool)
            .TaughtIn(District.Ryedale, TuitionTypes.Online));

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery("a-tuition-partner", ShowLocationsCovered: true));

        result.Should().NotBeNull();
        var coverage = result!.LocalAuthorityDistricts.Single(x => x.Name == "East Riding of Yorkshire");
        coverage.InSchool.Should().BeTrue();
        coverage.Online.Should().BeFalse();
    }

    [Fact]
    public async Task Shows_all_prices()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionTypes.InSchool, TuitionTypes.Online)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3English, s => s
                    .InSchool().Costing(12.34m).ForGroupSizes(2, 3))
                .Subject(Subjects.Id.KeyStage3Maths, s => s
                    .InSchool().Costing(56.78m).ForGroupSizes(2, 3)
                    .Online().Costing(56.78m).ForGroupSizes(3)))
        );

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery("a-tuition-partner", ShowFullPricing: true));

        result.Should().NotBeNull();
        result!.AllPrices[TuitionType.InSchool][KeyStage.KeyStage3]["English"].Should().BeEquivalentTo(
            new Dictionary<int, decimal>
            {
                {2, 12.34m},
                {3, 12.34m}
            });
        result!.AllPrices[TuitionType.InSchool][KeyStage.KeyStage3]["Maths"].Should().BeEquivalentTo(
            new Dictionary<int, decimal>
            {
                {2, 56.78m},
                {3, 56.78m}
            });
        result!.AllPrices[TuitionType.Online][KeyStage.KeyStage3]["Maths"].Should().BeEquivalentTo(
            new Dictionary<int, decimal>
            {
                {3, 56.78m}
            });
    }

    [Theory]
    [InlineData("a-tuition-partner", false)]
    [InlineData("bravo-learning", true)]
    public async Task Shows_send_status(string tuitionPartner, bool supportsSen)
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName(tuitionPartner)
            .WithSen(supportsSen)
        );
        var result = await Fixture.SendAsync(new GetTuitionPartnerQuery(tuitionPartner));
        result!.HasSenProvision.Should().Be(supportsSen);
    }

    [Theory]
    [InlineData("a-tuition-partner", 2, true)]
    [InlineData("a-tuition-partner", 3, true)]
    [InlineData("bravo-learning", 2, false)]
    public async Task Shows_price_variant_text(string tuitionPartner, int groupSize, bool hasVariation)
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionTypes.InSchool)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3English, s => s
                    .InSchool().Costing(12.34m).ForGroupSizes(2, 3))
                .Subject(Subjects.Id.KeyStage3Maths, s => s
                    .InSchool().Costing(56.78m).ForGroupSizes(2, 3)))
        );
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("bravo-learning")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionTypes.InSchool)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3English, s => s
                    .InSchool().Costing(12.34m).ForGroupSizes(2, 3))
                .Subject(Subjects.Id.KeyStage3Maths, s => s
                    .InSchool().Costing(12.34m).ForGroupSizes(2, 3)))
        );

        var result = await Fixture.SendAsync(new GetTuitionPartnerQuery(tuitionPartner));

        result.Should().NotBeNull();
        result!.Prices.Should().ContainKey(groupSize).WhoseValue.HasVariation.Should().Be(hasVariation);
    }

    [Fact]
    public async Task Shows_address_with_line_breaks()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .WithAddress("1 High Street\r\nBeautiful City\rThe County\nPostcode"));

        var result = await Fixture.SendAsync(new GetTuitionPartnerQuery("a-tuition-partner"));

        result.Should().NotBeNull();
        result!.Address.Should().ContainInOrder(
            "1 High Street", "Beautiful City", "The County", "Postcode");
    }
}