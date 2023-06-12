using Application.Common.Models.Enquiry.Build;
using Application.Validators.Enquiry.Build;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using UI.Pages.Enquiry.Build;
using EmailVerification = UI.Pages.Enquiry.Build.EmailVerification;

namespace Tests.Enquiry.Build;

[Collection(nameof(SliceFixture))]
public class EmailVerificationPageTests
{
    private readonly SliceFixture _fixture;
    private readonly EmailVerificationModelValidator _validator;

    public EmailVerificationPageTests(SliceFixture fixture)
    {
        _fixture = fixture;
        _validator = new EmailVerificationModelValidator();
    }

    private static EmailVerificationModel CreateModel(string passcode)
    {
        return new EmailVerificationModel { Passcode = passcode };
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_no_passcode(string passcode)
    {
        var model = CreateModel(passcode);
        var result = _validator!.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Passcode)
            .WithErrorMessage("Enter your passcode");
    }

    [Theory]
    [InlineData("ABC")]
    [InlineData("122rr7")]
    public void With_an_invalid_passcode(string passcode)
    {
        var model = CreateModel(passcode);
        var result = _validator!.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Passcode)
            .WithErrorMessage("The passcode must be a number");
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("999999")]
    public void With_a_valid_passcode(string passcode)
    {
        var model = CreateModel(passcode);
        var result = _validator!.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task With_a_valid_passcode_moves_to_next_page()
    {
        var model = CreateModel("999999");

        var result = await _fixture.GetPage<EmailVerification>().Execute(page =>
        {
            page.PageContext = GetPageContext();
            return page.OnPostAsync(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(TutoringLogistics));
    }

    [Fact]
    public async Task With_a_valid_passcode_moves_to_cya_page()
    {
        var model = CreateModel("999999");
        model.From = Domain.Enums.ReferrerList.CheckYourAnswers;

        var result = await _fixture.GetPage<EmailVerification>().Execute(page =>
        {
            page.PageContext = GetPageContext();
            return page.OnPostAsync(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(CheckYourAnswers));
    }

    private static PageContext GetPageContext()
    {
        var httpContext = new DefaultHttpContext();
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
        return new PageContext(actionContext);
    }
}