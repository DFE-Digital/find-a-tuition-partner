using Application.Common.Models;
using Application.Common.Structs;
using Application.Queries;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tests.TestData;
using UI.Models;
using KeyStage = Domain.Enums.KeyStage;
using TuitionPartner = UI.Pages.TuitionPartner;
using TuitionSetting = Domain.Enums.TuitionSetting;

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
            .Execute(page => page.OnGetAsync(new GetTuitionPartnerQueryModel(id)));
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData("A Tuition Partner")]
    [InlineData("A-Tuition-Partner")]
    public async Task Redirect_to_seo_url(string id)
    {
        var result = await Fixture.GetPage<TuitionPartner>()
            .Execute(page => page.OnGetAsync(new GetTuitionPartnerQueryModel(id)));
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
            .Execute(page => page.OnGetAsync(new GetTuitionPartnerQueryModel("not-found")));
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData("9")]
    [InlineData("this-tuition-partner")]
    public async Task Get_with_valid_id(string id)
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithId(9)
            .WithName("this-tuition-partner", "This Tuition Partner")
            .TaughtIn(District.Dacorum, TuitionSetting.FaceToFace)
            .WithSubjects(s => s
                .Subject(Subjects.Id.KeyStage1English, l => l
                    .FaceToFace().Costing(12m).ForGroupSizes(2))));

        var result = await Fixture.GetPage<TuitionPartner>()
            .Execute(page => page.OnGetAsync(new GetTuitionPartnerQueryModel(id)));
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
            .TaughtIn(District.EastRidingOfYorkshire, TuitionSetting.FaceToFace)
            .TaughtIn(District.NorthEastLincolnshire, TuitionSetting.FaceToFace, TuitionSetting.Online)
            .TaughtIn(District.Ryedale, TuitionSetting.Online)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3English, s => s
                    .FaceToFace().Costing(12.34m).ForGroupSizes(2, 3))
                .Subject(Subjects.Id.KeyStage3Maths, s => s
                    .FaceToFace().Costing(56.78m).ForGroupSizes(2, 3)
                    .Online().Costing(56.78m).ForGroupSizes(3)))
        );

        var result = await Fixture.SendAsync(new GetTuitionPartnerQuery(id));

        result!.Name.Should().Be("A Tuition Partner");
        result.Description.Should().Be("A Tuition Partner Description");
        result.Subjects.Should().BeEquivalentTo("Key stage 3: English and Maths");
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
    public async Task Shows_all_tuition_settings_no_district_specified()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionSetting.FaceToFace, TuitionSetting.Online));

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery("a-tuition-partner"));

        result!.TuitionSettings.Should().BeEquivalentTo("Online", "Face-to-face");
    }

    [Fact]
    public async Task Shows_all_tuition_settings_when_provided_in_district()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionSetting.FaceToFace, TuitionSetting.Online));

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery(
                    "a-tuition-partner")
            {
                SearchModel = new SearchModel()
                {
                    Postcode = District.EastRidingOfYorkshire.SamplePostcode
                }
            });

        result!.TuitionSettings.Should().BeEquivalentTo("Online", "Face-to-face");
    }

    [Fact]
    public async Task Shows_only_tuition_settings_provided_in_district()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.Ryedale, TuitionSetting.Online));

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery(
                    "a-tuition-partner")
            {
                SearchModel = new SearchModel()
                {
                    Postcode = District.Ryedale.SamplePostcode
                }
            });

        result!.TuitionSettings.Should().BeEquivalentTo("Online");
    }

    [Fact]
    public async Task Shows_all_tuition_settings_prices_when_provided_in_district()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.NorthEastLincolnshire, TuitionSetting.FaceToFace, TuitionSetting.Online)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3English, s => s
                    .FaceToFace().Costing(12.34m).ForGroupSizes(2, 3))
                .Subject(Subjects.Id.KeyStage3Maths, s => s
                    .FaceToFace().Costing(56.78m).ForGroupSizes(2, 3)
                    .Online().Costing(56.78m).ForGroupSizes(3))));

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery(
                    "a-tuition-partner")
            {
                SearchModel = new SearchModel()
                {
                    Postcode = District.NorthEastLincolnshire.SamplePostcode
                }
            });

        result!.Prices.Should().BeEquivalentTo(new Dictionary<int, GroupPrice>
        {
            {2, new(12.34m, 56.78m, null, null)},
            {3, new(12.34m, 56.78m, 56.78m, 56.78m)},
        });
    }

    [Fact]
    public async Task Shows_only_tuition_setting_prices_provided_in_district()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.NorthEastLincolnshire, TuitionSetting.FaceToFace)
            .TaughtIn(District.Ryedale, TuitionSetting.Online)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3Maths, s => s
                    .FaceToFace().Costing(33.44m).ForGroupSizes(2)
                    .Online().Costing(56.78m).ForGroupSizes(3))));

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery(
                    "a-tuition-partner")
            {
                SearchModel = new SearchModel()
                {
                    Postcode = District.Ryedale.SamplePostcode
                }
            });

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
            .TaughtIn(District.EastRidingOfYorkshire, TuitionSetting.FaceToFace)
            .TaughtIn(District.Ryedale, TuitionSetting.Online));

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery("a-tuition-partner", ShowLocationsCovered: true));

        result.Should().NotBeNull();
        var coverage = result!.LocalAuthorityDistricts.Single(x => x.Name == "East Riding of Yorkshire");
        coverage.FaceToFace.Should().BeTrue();
        coverage.Online.Should().BeFalse();
    }

    [Fact]
    public async Task Shows_all_prices()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionSetting.FaceToFace, TuitionSetting.Online)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3English, s => s
                    .FaceToFace().Costing(12.34m).ForGroupSizes(2, 3))
                .Subject(Subjects.Id.KeyStage3Maths, s => s
                    .FaceToFace().Costing(56.78m).ForGroupSizes(2, 3)
                    .Online().Costing(56.78m).ForGroupSizes(3)))
        );

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery("a-tuition-partner", ShowFullPricing: true));

        result.Should().NotBeNull();
        result!.AllPrices[TuitionSetting.FaceToFace][KeyStage.KeyStage3]["English"].Should().BeEquivalentTo(
            new Dictionary<int, decimal>
            {
                {2, 12.34m},
                {3, 12.34m}
            });
        result!.AllPrices[TuitionSetting.FaceToFace][KeyStage.KeyStage3]["Maths"].Should().BeEquivalentTo(
            new Dictionary<int, decimal>
            {
                {2, 56.78m},
                {3, 56.78m}
            });
        result!.AllPrices[TuitionSetting.Online][KeyStage.KeyStage3]["Maths"].Should().BeEquivalentTo(
            new Dictionary<int, decimal>
            {
                {3, 56.78m}
            });
    }

    [Fact]
    public async Task Shows_all_info()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionSetting.FaceToFace, TuitionSetting.Online)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3English, s => s
                    .FaceToFace().Costing(12.34m).ForGroupSizes(2, 3))
                .Subject(Subjects.Id.KeyStage3Maths, s => s
                    .FaceToFace().Costing(56.78m).ForGroupSizes(2, 3)
                    .Online().Costing(56.78m).ForGroupSizes(3)))
        );

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery("a-tuition-partner", ShowFullInfo: true));

        result.Should().NotBeNull();
        result!.AllPrices[TuitionSetting.FaceToFace][KeyStage.KeyStage3]["English"].Should().BeEquivalentTo(
            new Dictionary<int, decimal>
            {
                {2, 12.34m},
                {3, 12.34m}
            });

        var coverage = result!.LocalAuthorityDistricts.Single(x => x.Name == "East Riding of Yorkshire");
        coverage.FaceToFace.Should().BeTrue();
        coverage.Online.Should().BeTrue();

        result.ImportId.Should().NotBeNull();
    }

    [Fact]
    public async Task Not_show_all_info()
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionSetting.FaceToFace, TuitionSetting.Online)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3English, s => s
                    .FaceToFace().Costing(12.34m).ForGroupSizes(2, 3))
                .Subject(Subjects.Id.KeyStage3Maths, s => s
                    .FaceToFace().Costing(56.78m).ForGroupSizes(2, 3)
                    .Online().Costing(56.78m).ForGroupSizes(3)))
        );

        var result = await Fixture.SendAsync(
            new GetTuitionPartnerQuery("a-tuition-partner", ShowFullInfo: false));

        result.Should().NotBeNull();
        result!.ImportId.Should().BeNull();
    }


    [Theory]
    [InlineData("a-tuition-partner", 2, true)]
    [InlineData("a-tuition-partner", 3, true)]
    [InlineData("bravo-learning", 2, false)]
    public async Task Shows_price_variant_text(string tuitionPartner, int groupSize, bool hasVariation)
    {
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("a-tuition-partner")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionSetting.FaceToFace)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3English, s => s
                    .FaceToFace().Costing(12.34m).ForGroupSizes(2, 3))
                .Subject(Subjects.Id.KeyStage3Maths, s => s
                    .FaceToFace().Costing(56.78m).ForGroupSizes(2, 3)))
        );
        await Fixture.AddTuitionPartner(A.TuitionPartner
            .WithName("bravo-learning")
            .TaughtIn(District.EastRidingOfYorkshire, TuitionSetting.FaceToFace)
            .WithSubjects(c => c
                .Subject(Subjects.Id.KeyStage3English, s => s
                    .FaceToFace().Costing(12.34m).ForGroupSizes(2, 3))
                .Subject(Subjects.Id.KeyStage3Maths, s => s
                    .FaceToFace().Costing(12.34m).ForGroupSizes(2, 3)))
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
            .WithAddress("1 High Street\r\nBeautiful City\rThe County\nPostcode")
            .TaughtIn(District.Dacorum, TuitionSetting.FaceToFace)
            .WithSubjects(s => s
                .Subject(Subjects.Id.KeyStage1English, l => l
                    .FaceToFace().Costing(12m).ForGroupSizes(2))));

        var result = await Fixture.SendAsync(new GetTuitionPartnerQuery("a-tuition-partner"));

        result.Should().NotBeNull();
        result!.Address.Should().ContainInOrder(
            "1 High Street", "Beautiful City", "The County", "Postcode");
    }
}