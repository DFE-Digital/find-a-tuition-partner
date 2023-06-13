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
    private readonly TutoringLogisticsModelValidator _validator;

    public TutoringLogisticsTests(SliceFixture fixture)
    {
        _fixture = fixture;
        _validator = new TutoringLogisticsModelValidator();
    }

    private static TutoringLogisticsModel CreateModel(string data)
    {
        return new TutoringLogisticsModel { TutoringLogistics = data };
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_no_input(string data)
    {
        var model = CreateModel(data);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.TutoringLogistics);
    }

    [Fact]
    public void Has_less_than_max_data()
    {
        var model = CreateModel(new string('*', IntegerConstants.LargeTextAreaMaxCharacterSize - 1));
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_equal_to_max_data()
    {
        var model = CreateModel(new string('*', IntegerConstants.LargeTextAreaMaxCharacterSize));
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_more_than_max_data()
    {
        var model = CreateModel(new string('*', IntegerConstants.LargeTextAreaMaxCharacterSize + 1));
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.TutoringLogistics);
    }

    [Fact]
    public async Task With_a_valid_data_moves_to_next_page()
    {
        var model = CreateModel("Test 123");

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
        var model = CreateModel("Test 123");
        model.From = Domain.Enums.ReferrerList.CheckYourAnswers;

        var result = await _fixture.GetPage<TutoringLogistics>().Execute(page =>
        {
            return page.OnPostAsync(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(CheckYourAnswers));
    }
}