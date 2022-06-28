using Domain.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Domain.Tests.Validators;

public class TuitionPartnerValidatorTests
{
    private readonly TuitionPartnerValidator _validator;

    public TuitionPartnerValidatorTests()
    {
        _validator = new TuitionPartnerValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_no_name(string name)
    {
        var model = new TuitionPartner { Name = name };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_no_description(string description)
    {
        var model = new TuitionPartner { Description = description };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}