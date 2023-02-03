using Application.Common.Models;
using FluentValidation.TestHelper;
using Newtonsoft.Json;
using UI.Pages;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class ShortlistPageTests : CleanSliceFixture
{
    private readonly Mock<IMediator> _mediator;
    private readonly ShortlistModel _sut;
    public ShortlistPageTests(SliceFixture fixture) : base(fixture)
    {
        _mediator = new Mock<IMediator>();
        _sut = new ShortlistModel(_mediator.Object);
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

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task OnPostAddToShortlist_WhenCalledWithInvalidSeoUrl_ReturnExpectJson(string seoUrl)
    {
        var result = await _sut.OnPostAddToShortlist(GetAddToShortlistModel(seoUrl));

        AssertJsonResult(result, false);
        VerifyAddTuitionPartnerMediatorCall(0);
    }

    [Fact]
    public async Task OnPostAddToShortlist_WhenCalledWithSeoUrl_ReturnExpectJson()
    {
        _mediator.Setup(x => x.Send(It.IsAny<AddTuitionPartnersToShortlistCommand>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _sut.OnPostAddToShortlist(GetAddToShortlistModel("seoUrl"));

        AssertJsonResult(result, true);
        VerifyAddTuitionPartnerMediatorCall(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task OnPostRemoveFromShortlist_WhenCalledWithInvalidSeoUrl_ReturnExpectJson(string seoUrl)
    {
        var result = await _sut.OnPostRemoveFromShortlist(GetRemoveFromShortlistModel(seoUrl));

        AssertJsonResult(result, false);
        VerifyRemoveTuitionPartnerMediatorCall(0);
    }

    [Fact]
    public async Task OnPostRemoveFromShortlist_WhenCalledWithSeoUrl_ReturnExpectJson()
    {
        _mediator.Setup(x => x.Send(It.IsAny<RemoveShortlistedTuitionPartnerCommand>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _sut.OnPostRemoveFromShortlist(GetRemoveFromShortlistModel("seoUrl"));

        AssertJsonResult(result, true);
        VerifyRemoveTuitionPartnerMediatorCall(1);
    }

    private void AssertJsonResult(IActionResult result, bool isCallSuccessful)
    {
        result.Should().BeOfType<JsonResult>();
        var jsonResult = result as JsonResult;
        var json = JsonConvert.SerializeObject(jsonResult?.Value);
        var resultData = JsonConvert.DeserializeObject<ShortlistedTuitionPartnerResult>(json);
        resultData?.IsCallSuccessful.Should().Be(isCallSuccessful);
    }

    private void VerifyRemoveTuitionPartnerMediatorCall(int numberOfTimes) => _mediator.Verify(m =>
        m.Send(It.IsAny<RemoveShortlistedTuitionPartnerCommand>(), default), Times.Exactly(numberOfTimes));

    private void VerifyAddTuitionPartnerMediatorCall(int numberOfTimes) => _mediator.Verify(m =>
        m.Send(It.IsAny<AddTuitionPartnersToShortlistCommand>(), default), Times.Exactly(numberOfTimes));

    private AddToShortlistModel GetAddToShortlistModel(string seoUrl, int totalShortlistedTuitionPartners = 1)
    {
        var addToShortlistModel = new AddToShortlistModel()
        {
            SeoUrl = seoUrl,
            TotalShortlistedTuitionPartners = totalShortlistedTuitionPartners
        };

        return addToShortlistModel;
    }

    private RemoveFromShortlistModel GetRemoveFromShortlistModel(string seoUrl, int totalShortlistedTuitionPartners = 1)
    {
        var removeFromShortlistModel = new RemoveFromShortlistModel()
        {
            SeoUrl = seoUrl,
            TotalShortlistedTuitionPartners = totalShortlistedTuitionPartners
        };

        return removeFromShortlistModel;
    }

    private static object? GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName)?.GetValue(src, null);
    }
}