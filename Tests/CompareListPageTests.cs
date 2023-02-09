using Application.Common.Models;
using FluentValidation.TestHelper;
using Newtonsoft.Json;
using UI.Pages;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class CompareListPageTests : CleanSliceFixture
{
    private readonly Mock<IMediator> _mediator;
    private readonly CompareList _sut;
    public CompareListPageTests(SliceFixture fixture) : base(fixture)
    {
        _mediator = new Mock<IMediator>();
        _sut = new CompareList(_mediator.Object);
    }

    [Theory]
    [InlineData("not a postcode")]
    [InlineData("ABCDD EFG")]
    public void With_an_invalid_postcode(string postcode)
    {
        var model = new CompareList.Query() { Postcode = postcode };
        var result = new CompareList.Validator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Postcode)
            .WithErrorMessage("Enter a real postcode");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void With_a_blank_or_null_postcode(string postcode)
    {
        var model = new CompareList.Query() { Postcode = postcode };
        var result = new CompareList.Validator().TestValidate(model);
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
        var query = new CompareList.Query() { Postcode = postcode };
        var result = await Fixture.GetPage<CompareList>().Execute(async page => await page.OnGet(query));
        result.Should().BeOfType<RedirectToPageResult>();
        var pageName = GetPropValue(result, pageNameProp);
        pageName.Should().Be(nameof(SearchResults));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task OnPostAddToCompareList_WhenCalledWithInvalidSeoUrl_ReturnExpectJson(string seoUrl)
    {
        var result = await _sut.OnPostAddToCompareList(GetAddToCompareListModel(seoUrl));

        AssertJsonResult(result, false);
        VerifyAddTuitionPartnerMediatorCall(0);
    }

    [Fact]
    public async Task OnPostAddToCompareList_WhenCalledWithSeoUrl_ReturnExpectJson()
    {
        _mediator.Setup(x => x.Send(It.IsAny<AddTuitionPartnersToCompareListCommand>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _sut.OnPostAddToCompareList(GetAddToCompareListModel("seoUrl"));

        AssertJsonResult(result, true);
        VerifyAddTuitionPartnerMediatorCall(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task OnPostRemoveFromCompareList_WhenCalledWithInvalidSeoUrl_ReturnExpectJson(string seoUrl)
    {
        var result = await _sut.OnPostRemoveFromCompareList(GetRemoveFromCompareListModel(seoUrl));

        AssertJsonResult(result, false);
        VerifyRemoveTuitionPartnerMediatorCall(0);
    }

    [Fact]
    public async Task OnPostRemoveFromCompareList_WhenCalledWithSeoUrl_ReturnExpectJson()
    {
        _mediator.Setup(x => x.Send(It.IsAny<RemoveCompareListedTuitionPartnerCommand>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _sut.OnPostRemoveFromCompareList(GetRemoveFromCompareListModel("seoUrl"));

        AssertJsonResult(result, true);
        VerifyRemoveTuitionPartnerMediatorCall(1);
    }

    private void AssertJsonResult(IActionResult result, bool isCallSuccessful)
    {
        result.Should().BeOfType<JsonResult>();
        var jsonResult = result as JsonResult;
        var json = JsonConvert.SerializeObject(jsonResult?.Value);
        var resultData = JsonConvert.DeserializeObject<CompareListedTuitionPartnerResult>(json);
        resultData?.IsCallSuccessful.Should().Be(isCallSuccessful);
    }

    private void VerifyRemoveTuitionPartnerMediatorCall(int numberOfTimes) => _mediator.Verify(m =>
        m.Send(It.IsAny<RemoveCompareListedTuitionPartnerCommand>(), default), Times.Exactly(numberOfTimes));

    private void VerifyAddTuitionPartnerMediatorCall(int numberOfTimes) => _mediator.Verify(m =>
        m.Send(It.IsAny<AddTuitionPartnersToCompareListCommand>(), default), Times.Exactly(numberOfTimes));

    private AddToCompareListModel GetAddToCompareListModel(string seoUrl, int totalCompareListedTuitionPartners = 1)
    {
        var addToCompareListModel = new AddToCompareListModel()
        {
            SeoUrl = seoUrl,
            TotalCompareListedTuitionPartners = totalCompareListedTuitionPartners
        };

        return addToCompareListModel;
    }

    private RemoveFromCompareListModel GetRemoveFromCompareListModel(string seoUrl, int totalCompareListedTuitionPartners = 1)
    {
        var removeFromCompareListModel = new RemoveFromCompareListModel()
        {
            SeoUrl = seoUrl,
            TotalCompareListedTuitionPartners = totalCompareListedTuitionPartners
        };

        return removeFromCompareListModel;
    }

    private static object? GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName)?.GetValue(src, null);
    }
}