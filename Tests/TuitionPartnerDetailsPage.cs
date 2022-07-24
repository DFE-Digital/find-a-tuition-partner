using Domain;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KeyStage = UI.Pages.KeyStage;
using TuitionPartner = UI.Pages.TuitionPartner;
using TuitionType = UI.Pages.TuitionType;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class TuitionPartnerDetailsPage : CleanSliceFixture
{
    public TuitionPartnerDetailsPage(SliceFixture fixture) : base(fixture)
    {
        
    }

    private string? _district1Code;
    private string? _district9Code;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        await Fixture.ExecuteDbContextAsync(async db =>
        {
            _district1Code = db.LocalAuthorityDistricts.Find(1)!.Code;
            _district9Code = db.LocalAuthorityDistricts.Find(9)!.Code;

            db.TuitionPartners.Add(new Domain.TuitionPartner
            {
                Id = 1,
                SeoUrl = "a-tuition-partner",
                Name = "A Tuition Partner",
                Website = "https://a-tuition-partner.testdata/ntp",
                Description = "A Tuition Partner Description",
                PhoneNumber = "0123456789",
                Email = "ntp@a-tuition-partner.testdata",
                LocalAuthorityDistrictCoverage = new List<LocalAuthorityDistrictCoverage>
                {
                    new() { TuitionTypeId = (int)TuitionTypes.InSchool, LocalAuthorityDistrictId = 1 },
                    new() { TuitionTypeId = (int)TuitionTypes.Online, LocalAuthorityDistrictId = 9 },
                },
                SubjectCoverage = new List<SubjectCoverage>
                {
                    new() { TuitionTypeId = (int)TuitionTypes.InSchool, SubjectId = Subjects.Id.KeyStage3English },
                    new() { TuitionTypeId = (int)TuitionTypes.InSchool, SubjectId = Subjects.Id.KeyStage3Maths },
                },
                Prices = new List<Price>
                {
                    new() { TuitionTypeId = (int)TuitionTypes.InSchool, SubjectId = Subjects.Id.KeyStage3English, GroupSize = 2, HourlyRate = 12.34m },
                    new() { TuitionTypeId = (int)TuitionTypes.InSchool, SubjectId = Subjects.Id.KeyStage3English, GroupSize = 3, HourlyRate = 12.34m },
                    new() { TuitionTypeId = (int)TuitionTypes.InSchool, SubjectId = Subjects.Id.KeyStage3Maths, GroupSize = 2, HourlyRate = 56.78m },
                    new() { TuitionTypeId = (int)TuitionTypes.InSchool, SubjectId = Subjects.Id.KeyStage3Maths, GroupSize = 3, HourlyRate = 56.78m },
                }
            });

            await db.SaveChangesAsync();
        });
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Get_with_null_or_whitespace_id(string id)
    {
        var result = await Fixture.GetPage<TuitionPartner>()
            .Execute(page => page.OnGetAsync(new TuitionPartner.Query(id)));
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData("A Tuition Partner")]
    [InlineData("A-Tuition-Partner")]
    public async Task Redirect_to_seo_url(string id)
    {
        var result = await Fixture.GetPage<TuitionPartner>()
            .Execute(page => page.OnGetAsync(new TuitionPartner.Query(id)));
        result.Should().BeAssignableTo<RedirectToPageResult>()
            .Which.RouteValues.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                {"Id", "a-tuition-partner" }
            });
    }

    [Fact]
    public async Task Tuition_partner_not_found()
    {
        var result = await Fixture.GetPage<TuitionPartner>()
            .Execute(page => page.OnGetAsync(new TuitionPartner.Query("not-found")));
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData("1")]
    [InlineData("a-tuition-partner")]
    public async Task Get_with_valid_id(string id)
    {
        var result = await Fixture.GetPage<TuitionPartner>()
            .Execute(page => page.OnGetAsync(new TuitionPartner.Query(id)));
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task Returns_null_when_not_found()
    {
        var result = await Fixture.SendAsync(new TuitionPartner.Query("not-found"));

        result.Should().BeNull();
    }

    [Theory]
    [InlineData("1")]
    [InlineData("a-tuition-partner")]
    public async Task Returns_tuition_partner_details(string id)
    {
        var result = await Fixture.SendAsync(new TuitionPartner.Query(id));

        result.Name.Should().Be("A Tuition Partner");
        result.Description.Should().Be("A Tuition Partner Description");
        result.Subjects.Should().BeEquivalentTo("Key stage 3 - English and Maths");
        result.Ratios.Should().BeEquivalentTo("1 to 2", "1 to 3");
        result.Prices.Should().BeEquivalentTo(new Dictionary<int, TuitionPartner.GroupPrice>
        {
            { 2, new (12.34m, 56.78m, null, null) },
            { 3, new (12.34m, 56.78m, null, null) }
        });
        result.Website.Should().Be("https://a-tuition-partner.testdata/ntp");
        result.PhoneNumber.Should().Be("0123456789");
        result.EmailAddress.Should().Be("ntp@a-tuition-partner.testdata");
    }

    [Fact]
    public async Task Shows_all_tuition_types_no_district_specified()
    {
        var result = await Fixture.SendAsync(
            new TuitionPartner.Query("a-tuition-partner"));

        result!.TuitionTypes.Should().BeEquivalentTo("Online", "In School");
    }

    [Fact]
    public async Task Shows_all_tuition_types_when_provided_in_district()
    {
        var result = await Fixture.SendAsync(
            new TuitionPartner.Query("a-tuition-partner", LocalAuthorityDistrictCode: _district1Code));

        result!.TuitionTypes.Should().BeEquivalentTo("Online", "In School");
    }

    [Fact]
    public async Task Shows_only_tuition_types_provided_in_district()
    {
        var result = await Fixture.SendAsync(
            new TuitionPartner.Query("a-tuition-partner", LocalAuthorityDistrictCode: _district9Code));

        result!.TuitionTypes.Should().BeEquivalentTo("Online");
    }

    [Fact]
    public async Task Shows_all_locations()
    {
        var result = await Fixture.SendAsync(new TuitionPartner.Query("a-tuition-partner", ShowLocationsCovered: true));

        result.Should().NotBeNull();
        var coverage = result!.LocalAuthorityDistricts.Single(x => x.Name == "East Riding of Yorkshire");
        coverage.InSchool.Should().BeTrue();
        coverage.Online.Should().BeFalse();
    }

    [Fact]
    public async Task Shows_all_prices()
    {
        var result = await Fixture.SendAsync(new TuitionPartner.Query("a-tuition-partner", ShowFullPricing: true));

        result.Should().NotBeNull();
        result!.AllPrices[TuitionType.InSchool][KeyStage.KeyStage3]["English"].Should().BeEquivalentTo(
            new Dictionary<int, decimal>
            {
                { 2, 12.34m },
                { 3, 12.34m }
            });
        result!.AllPrices[TuitionType.InSchool][KeyStage.KeyStage3]["Maths"].Should().BeEquivalentTo(
            new Dictionary<int, decimal>
            {
                { 2, 56.78m },
                { 3, 56.78m }
            });
    }
}