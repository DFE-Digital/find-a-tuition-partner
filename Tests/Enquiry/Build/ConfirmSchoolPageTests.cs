using Application.Common.Models.Enquiry.Build;
using Application.Validators.Enquiry.Build;
using FluentValidation.TestHelper;
using UI.Pages.Enquiry.Build;
using ConfirmSchool = UI.Pages.Enquiry.Build.ConfirmSchool;

namespace Tests.Enquiry.Build;

[Collection(nameof(SliceFixture))]
public class ConfirmSchoolPageTests
{
    private readonly SliceFixture _fixture;

    public ConfirmSchoolPageTests(SliceFixture fixture) => _fixture = fixture;

    [Fact]
    public void With_single_true_school_valid()
    {
        var model = new ConfirmSchoolModel { HasSingleSchool = true, ConfirmedIsSchool = true };
        var result = new ConfirmSchoolModelValidator().TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void With_single_false_school_valid()
    {
        var model = new ConfirmSchoolModel { HasSingleSchool = true, ConfirmedIsSchool = false };
        var result = new ConfirmSchoolModelValidator().TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void With_single_school_invalid()
    {
        var model = new ConfirmSchoolModel { HasSingleSchool = true, ConfirmedIsSchool = null };
        var result = new ConfirmSchoolModelValidator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmedIsSchool)
            .WithErrorMessage("Select to confirm if this is your school or not");
    }

    [Fact]
    public void With_multiple_school_valid()
    {
        var model = new ConfirmSchoolModel { HasSingleSchool = false, SchoolId = 1 };
        var result = new ConfirmSchoolModelValidator().TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void With_multiple_school_invalid()
    {
        var model = new ConfirmSchoolModel { HasSingleSchool = false, SchoolId = null };
        var result = new ConfirmSchoolModelValidator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.SchoolId)
            .WithErrorMessage("Select to confirm which school you work for");
    }

    [Fact]
    public async Task With_a_valid_data_moves_to_next_page()
    {
        var model = new ConfirmSchoolModel { HasSingleSchool = false, SchoolId = 1 };

        var result = await _fixture.GetPage<ConfirmSchool>().Execute(page =>
        {
            return page.OnPostAsync(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(EnquirerEmail));
    }

    [Fact]
    public async Task With_a_valid_data_moves_to_cya_page()
    {
        var model = new ConfirmSchoolModel { HasSingleSchool = false, SchoolId = 1, From = Domain.Enums.ReferrerList.CheckYourAnswers };

        var result = await _fixture.GetPage<ConfirmSchool>().Execute(page =>
        {
            return page.OnPostAsync(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(CheckYourAnswers));
    }
}