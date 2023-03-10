using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using Application.Validators.Enquiry.Build;
using FluentValidation.TestHelper;
using UI.Pages.Enquiry.Build;

namespace Tests.Enquiry.Build;

[Collection(nameof(SliceFixture))]
public class SendRequirementsTests
{
    private readonly SliceFixture _fixture;

    public SendRequirementsTests(SliceFixture fixture) => _fixture = fixture;

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_no_input(string data)
    {
        var model = new SendRequirementsModel { SendRequirements = data };
        var result = new SendRequirementsModelValidator().TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_less_than_max_data()
    {
        var model = new SendRequirementsModel { SendRequirements = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize - 1) };
        var result = new SendRequirementsModelValidator().TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_equal_to_max_data()
    {
        var model = new SendRequirementsModel { SendRequirements = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize) };
        var result = new SendRequirementsModelValidator().TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_more_than_to_max_data()
    {
        var model = new SendRequirementsModel { SendRequirements = new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize + 1) };
        var result = new SendRequirementsModelValidator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.SendRequirements);
    }

    [Fact]
    public async Task With_a_valid_data_moves_to_next_page()
    {
        var model = new SendRequirementsModel { SendRequirements = "Test 123" };

        var result = await _fixture.GetPage<SendRequirements>().Execute(page =>
        {
            return page.OnGetSubmit(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(AdditionalInformation));
    }

    [Fact]
    public async Task With_a_valid_data_moves_to_cya_page()
    {
        var model = new SendRequirementsModel { SendRequirements = "Test 123", From = Domain.Enums.ReferrerList.CheckYourAnswers };

        var result = await _fixture.GetPage<SendRequirements>().Execute(page =>
        {
            return page.OnGetSubmit(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(CheckYourAnswers));
    }
}