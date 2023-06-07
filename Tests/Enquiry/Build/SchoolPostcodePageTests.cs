using Application.Common.Models.Enquiry.Build;
using Application.Validators.Enquiry.Build;
using FluentValidation.TestHelper;
using Tests.TestData;
using UI.Pages.Enquiry.Build;
using SchoolPostcode = UI.Pages.Enquiry.Build.SchoolPostcode;

namespace Tests.Enquiry.Build;

[Collection(nameof(SliceFixture))]
public class SchoolPostcodePageTests
{
    private readonly SliceFixture _fixture;

    public SchoolPostcodePageTests(SliceFixture fixture) => _fixture = fixture;

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_no_postcode(string postcode)
    {
        var model = new SchoolPostcodeModel { SchoolPostcode = postcode };
        var result = new SchoolPostcodeModelValidator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.SchoolPostcode)
            .WithErrorMessage("Enter a postcode");
    }

    [Theory]
    [InlineData("not a postcode")]
    public void With_an_invalid_postcode(string postcode)
    {
        var model = new SchoolPostcodeModel { SchoolPostcode = postcode };
        var result = new SchoolPostcodeModelValidator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.SchoolPostcode)
            .WithErrorMessage("Enter a real postcode");
    }

    [Theory]
    [InlineData("LL58 8EP")]
    [InlineData("DD1 4NP")]
    public void With_a_valid_postcode(string postcode)
    {
        var model = new SchoolPostcodeModel { SchoolPostcode = postcode };
        var result = new SchoolPostcodeModelValidator().TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task With_a_valid_postcode_moves_to_next_page()
    {
        var model = new SchoolPostcodeModel { SchoolPostcode = District.EastRidingOfYorkshire.SamplePostcode };

        var result = await _fixture.GetPage<SchoolPostcode>().Execute(page =>
        {
            return page.OnPostAsync(model);
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be(nameof(ConfirmSchool));
    }
}