using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using Application.Validators.Enquiry.Build;
using FluentValidation.TestHelper;
using UI.Pages.Enquiry.Build;

namespace Tests.Enquiry.Build;

[Collection(nameof(SliceFixture))]
public class TutoringLogisticsTests
{
    private readonly SliceFixture _fixture;

    public TutoringLogisticsTests(SliceFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_no_input(string data)
    {
        var model = new TutoringLogisticsModel { TutoringLogistics = data };
        var result = new TutoringLogisticsModelValidator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.TutoringLogistics);
    }

    [Fact]
    public void Has_less_than_max_data()
    {
        var model = new TutoringLogisticsModel { TutoringLogistics = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize - 1) };
        var result = new TutoringLogisticsModelValidator().TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_equal_to_max_data()
    {
        var model = new TutoringLogisticsModel { TutoringLogistics = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize) };
        var result = new TutoringLogisticsModelValidator().TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_more_than_max_data()
    {
        var model = new TutoringLogisticsModel { TutoringLogistics = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize + 1) };
        var result = new TutoringLogisticsModelValidator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.TutoringLogistics);
    }

    [Fact]
    public async Task With_a_valid_data_moves_to_next_page()
    {
        var model = new TutoringLogisticsModel { TutoringLogistics = "Test 123" };

        var result = await _fixture.GetPage<TutoringLogistics>().Execute(page =>
        {
            return page.OnPostAsync(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(SENDRequirements));
    }

    [Fact]
    public async Task With_a_valid_data_moves_to_cya_page()
    {
        var model = new TutoringLogisticsModel { TutoringLogistics = "Test 123", From = Domain.Enums.ReferrerList.CheckYourAnswers };

        var result = await _fixture.GetPage<TutoringLogistics>().Execute(page =>
        {
            return page.OnPostAsync(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(CheckYourAnswers));
    }
}