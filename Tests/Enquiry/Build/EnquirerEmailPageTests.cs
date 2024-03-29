﻿using Application.Common.Models.Enquiry.Build;
using Application.Validators.Enquiry.Build;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using UI.Pages.Enquiry.Build;
using EnquirerEmail = UI.Pages.Enquiry.Build.EnquirerEmail;

namespace Tests.Enquiry.Build;

[Collection(nameof(SliceFixture))]
public class EnquirerEmailPageTests
{
    private readonly SliceFixture _fixture;
    private readonly EnquirerEmailModelValidator _validator;

    public EnquirerEmailPageTests(SliceFixture fixture)
    {
        _fixture = fixture;
        _validator = new EnquirerEmailModelValidator();
    }

    private static EnquirerEmailModel CreateModel(string data)
    {
        return new EnquirerEmailModel { Email = data };
    }


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_no_email(string email)
    {
        var model = CreateModel(email);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Enter an email address");
    }

    [Theory]
    [InlineData("not an email address")]
    [InlineData("ABC")]
    [InlineData("abc@")]
    [InlineData("@hhhh")]
    [InlineData("not an email address @ some place")]
    [InlineData("Joe Smith <email@example.com>")]
    [InlineData("email.example.com")]
    [InlineData("email@example@example.com")]
    [InlineData("email@example.com (Joe Smith)")]
    public void With_an_invalid_email(string email)
    {
        var model = CreateModel(email);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("You must enter an email address in the correct format.  Emails are usually in a format, like, username@example.com");
    }

    [Theory]
    [InlineData("email@example.com")]
    [InlineData("firstname.lastname@example.com")]
    [InlineData("email@subdomain.example.com")]
    [InlineData("firstname+lastname@example.com")]
    [InlineData("email@123.123.123.123")]
    [InlineData("email@[123.123.123.123]")]
    [InlineData("1234567890@example.com")]
    [InlineData("email@example-one.com")]
    [InlineData("_______@example.com")]
    [InlineData("firstname-lastname@example.com")]
    [InlineData("firstname'lastname@example.com")]
    [InlineData("firstname'lastname@example")]
    [InlineData("firstname'lastname@example_com")]
    public void With_a_valid_email(string email)
    {
        var model = CreateModel(email);
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("email@example.com")]
    public async Task With_a_valid_email_moves_to_next_page(string email)
    {
        var model = CreateModel(email);

        var result = await _fixture.GetPage<EnquirerEmail>().Execute(page =>
        {
            page.PageContext = GetPageContext();
            return page.OnPostAsync(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(EmailVerification));
    }

    [Theory]
    [InlineData("email@example.com")]
    public async Task With_a_valid_email_moves_to_cya_page(string email)
    {
        var model = CreateModel(email);
        model.From = Domain.Enums.ReferrerList.CheckYourAnswers;

        var result = await _fixture.GetPage<EnquirerEmail>().Execute(page =>
        {
            page.PageContext = GetPageContext();
            return page.OnPostAsync(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(EmailVerification));
    }

    private static PageContext GetPageContext()
    {
        var httpContext = new DefaultHttpContext();
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
        return new PageContext(actionContext);
    }
}