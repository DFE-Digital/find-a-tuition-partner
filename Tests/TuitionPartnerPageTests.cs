using UI.Pages;

namespace Tests;

public class TuitionPartnerPageTests
{
    private readonly Mock<ILogger<TuitionPartner>> _logger;
    private readonly Mock<IMediator> _mediator;

    public TuitionPartnerPageTests()
    {
        _logger = new Mock<ILogger<TuitionPartner>>();
        _mediator = new Mock<IMediator>();
    }

    private TuitionPartner GetTuitionPartner() => new(_logger.Object, _mediator.Object);

    private string GetSearchModel =>
        @"{""From"":0,""Name"":null,""Postcode"":""DE1 1RY"",""Subjects"":[""KeyStage1-English""],""TuitionType"":0,""KeyStages"":[1]}";

    private void VerifyRemoveTuitionPartnerMediatorCall(int numberOfTimes) => _mediator.Verify(m =>
        m.Send(It.IsAny<RemoveShortlistedTuitionPartnerCommand>(), default), Times.Exactly(numberOfTimes));

    private void VerifyAddTuitionPartnerMediatorCall(int numberOfTimes) => _mediator.Verify(m =>
        m.Send(It.IsAny<AddTuitionPartnersToShortlistCommand>(), default), Times.Exactly(numberOfTimes));

    private void AssertRedirect(IActionResult result)
    {
        result.Should().BeOfType<RedirectToPageResult>();
        var redirect = result as RedirectToPageResult;
        redirect?.PageName.Should().Be("TuitionPartner");
    }

    [Theory]
    [InlineData("", "searchModel")]
    [InlineData(" ", "searchModel")]
    [InlineData(null, "searchModel")]
    [InlineData("seoUrl", "")]
    [InlineData("seoUrl", " ")]
    [InlineData("seoUrl", null)]
    public async Task OnPostUpdateShortlist_WhenCalledWithInvalidParameters_ThrowsArgumentException(string seoUrl,
        string searchModel)
    {
        var sut = GetTuitionPartner();

        await Assert.ThrowsAsync<ArgumentException>(() => sut.OnPostUpdateShortlist(seoUrl, searchModel));
    }

    [Fact]
    public async Task OnPostUpdateShortlist_WhenShortlistedCheckboxIsWhitespaceOrNull_MakesTheRightMediatorCall()
    {
        var sut = GetTuitionPartner();

        var result = await sut.OnPostUpdateShortlist("seoUrl", GetSearchModel);

        AssertRedirect(result);
        VerifyRemoveTuitionPartnerMediatorCall(1);
    }

    [Fact]
    public async Task OnPostUpdateShortlist_WhenShortlistedCheckboxIsNotWhitespaceOrNull_MakesTheRightMediatorCall()
    {
        var sut = GetTuitionPartner();
        sut.ShortlistedCheckbox = "ShortlistedCheckbox";

        var result = await sut.OnPostUpdateShortlist("seoUrl", GetSearchModel);

        AssertRedirect(result);
        VerifyAddTuitionPartnerMediatorCall(1);
    }
}