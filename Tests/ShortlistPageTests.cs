using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc;
using UI.Pages;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class ShortlistPageTests : CleanSliceFixture
{
    public ShortlistPageTests(SliceFixture fixture) : base(fixture)
    {
    }

    [Theory]
    [InlineData("not a postcode")]
    [InlineData("ABCDD EFG")]
    public void With_an_invalid_postcode(string postcode)
    {
        var model = new ShortlistModel.Query() { Postcode = postcode };
        var result = new ShortlistModel.Validator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Postcode)
            .WithErrorMessage("Enter a real postcode");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void With_a_blank_or_null_postcode(string postcode)
    {
        var model = new ShortlistModel.Query() { Postcode = postcode };
        var result = new ShortlistModel.Validator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Postcode)
            .WithErrorMessage("Enter a postcode");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("not a postcode")]
    [InlineData("ABCDD EFG")]
    public async Task With_a_blank_or_invalid_postcode_user_should_be_redirect_to_search_results_page(string postcode)
    {
        const string pageNameProp = "PageName";
        var query = new ShortlistModel.Query() { Postcode = postcode };
        var result = await Fixture.GetPage<ShortlistModel>().Execute(async page => await page.OnGet(query));
        result.Should().BeOfType<RedirectToPageResult>();
        var pageName = GetPropValue(result, pageNameProp);
        pageName.Should().Be(nameof(SearchResults));
    }

    private static object? GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName)?.GetValue(src, null);
    }
}