using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using Application.Validators.Enquiry.Build;
using FluentValidation.TestHelper;
using UI.Pages.Enquiry.Build;

namespace Tests.Enquiry.Build;

[Collection(nameof(SliceFixture))]
public class AdditionalInformationTests
{
    private readonly SliceFixture _fixture;
    private readonly AdditionalInformationModelValidator _validator;

    public AdditionalInformationTests(SliceFixture fixture)
    {
        _fixture = fixture;
        _validator = new AdditionalInformationModelValidator();
    }

    private static AdditionalInformationModel CreateModel(string data)
    {
        return new AdditionalInformationModel { AdditionalInformation = data };
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_no_input(string data)
    {
        var model = CreateModel(data);
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_less_than_max_data()
    {
        var model = CreateModel(new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize - 1));
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_equal_to_max_data()
    {
        var model = CreateModel(new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize));
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_more_than_max_data()
    {
        var model = CreateModel(new string('*', IntegerConstants.EnquiryQuestionsMaxCharacterSize + 1));
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AdditionalInformation);
    }

    [Fact]
    public async Task With_a_valid_data_moves_to_next_page()
    {
        var model = CreateModel("Test 123");

        var result = await _fixture.GetPage<AdditionalInformation>().Execute(page =>
        {
            return page.OnPostAsync(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(CheckYourAnswers));
    }

    [Fact]
    public async Task With_a_valid_data_moves_to_cya_page()
    {
        var model = CreateModel("Test 123");
        model.From = Domain.Enums.ReferrerList.CheckYourAnswers;

        var result = await _fixture.GetPage<AdditionalInformation>().Execute(page =>
        {
            return page.OnPostAsync(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(CheckYourAnswers));
    }
}