using Application.Common.Models.Enquiry.Build;
using Application.Validators.Enquiry.Build;
using FluentValidation.TestHelper;
using UI.Pages.Enquiry.Build;
using EnquirerEmail = UI.Pages.Enquiry.Build.EnquirerEmail;

namespace Tests.Enquiry.Build;

[Collection(nameof(SliceFixture))]
public class EnquirerEmailPageTests
{
    private readonly SliceFixture _fixture;

    public EnquirerEmailPageTests(SliceFixture fixture) => _fixture = fixture;

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_no_email(string email)
    {
        var model = new EnquirerEmailModel { Email = email };
        var result = new EnquirerEmailModelValidator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email address is required");
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
        var model = new EnquirerEmailModel { Email = email };
        var result = new EnquirerEmailModelValidator().TestValidate(model);
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
        var model = new EnquirerEmailModel { Email = email };
        var result = new EnquirerEmailModelValidator().TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("email@example.com")]
    public async Task With_a_valid_email_moves_to_next_page(string email)
    {
        var model = new EnquirerEmailModel { Email = email };

        var result = await _fixture.GetPage<EnquirerEmail>().Execute(page =>
        {
            return page.OnGetSubmit(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(TutoringLogistics));
    }

    [Theory]
    [InlineData("email@example.com")]
    public async Task With_a_valid_email_moves_to_cya_page(string email)
    {
        var model = new EnquirerEmailModel { Email = email, From = Domain.Enums.ReferrerList.CheckYourAnswers };

        var result = await _fixture.GetPage<EnquirerEmail>().Execute(page =>
        {
            return page.OnGetSubmit(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(CheckYourAnswers));
    }
}