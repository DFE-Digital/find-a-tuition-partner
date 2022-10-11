using Microsoft.AspNetCore.Mvc;
using Tests.TestData;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class TuitionPartnerLogoPage : SliceFixture
{
    [Theory]
    [InlineData("first", "aGK=")]
    [InlineData("second", "bG9nbw==")]
    public async Task Returns_logo_data(string logo, string content)
    {
        await AddTuitionPartner(A.TuitionPartner
            .WithName(logo).WithLogo(content));

        var result = await GetPage<UI.Pages.TuitionPartnerLogo>()
            .Execute(page => page.OnGet(logo));

        result.Should().BeOfType<FileContentResult>()
            .Which.FileContents.Should().BeEquivalentTo(Convert.FromBase64String(content));
    }

    [Theory]
    [InlineData("first", ".svg", "image/svg+xml")]
    [InlineData("second", ".png", "image/png")]
    public async Task Returns_correct_mime_type(string logo, string extension, string mime)
    {
        await AddTuitionPartner(A.TuitionPartner
            .WithName(logo).WithLogo("aGK=", extension));

        var result = await GetPage<UI.Pages.TuitionPartnerLogo>()
            .Execute(page => page.OnGet(logo));

        result.Should().BeOfType<FileContentResult>()
            .Which.ContentType.Should().Be(mime);
    }
}