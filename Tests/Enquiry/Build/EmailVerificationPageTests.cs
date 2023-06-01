using System;
using System.Collections.Generic;
using System.Linq;
using Application.Common.Models.Enquiry.Build;
using Application.Validators.Enquiry.Build;
using FluentValidation.TestHelper;
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

    private EmailVerificationModel CreateModel(string passcode)
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
        var model = new EmailVerificationModel { Passcode = passcode };
        var result = _validator!.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Passcode)
            .WithErrorMessage("The passcode must be a number");
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("999999")]
    public void With_a_valid_passcode(string passcode)
    {
        var model = new EmailVerificationModel { Passcode = passcode };
        var result = _validator!.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}