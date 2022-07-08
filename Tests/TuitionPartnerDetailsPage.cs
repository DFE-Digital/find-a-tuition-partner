using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UI.Pages.FindATuitionPartner;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class TuitionPartnerDetailsPage : CleanSliceFixture
{
    public TuitionPartnerDetailsPage(SliceFixture fixture) : base(fixture)
    {
        
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        await Fixture.ExecuteDbContextAsync(async db =>
        {
            db.TuitionPartners.Add(new Domain.TuitionPartner
            {
                Id = 1,
                Name = "A Tuition Partner",
                SeoUrl = "a-tuition-partner",
                Website = "https://a-tuition-partner.testdata"
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

    [Fact]
    public async Task Get_with_numerical_id()
    {
        var result = await Fixture.GetPage<TuitionPartner>()
            .Execute(page => page.OnGetAsync(new TuitionPartner.Query("1")));
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task Get_with_seo_id()
    {
        var result = await Fixture.GetPage<TuitionPartner>()
            .Execute(page => page.OnGetAsync(new TuitionPartner.Query("a-tuition-partner")));
        result.Should().BeOfType<PageResult>();
    }
}