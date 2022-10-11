using Domain.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Domain.Tests.Validators;

public class SchoolValidatorTests
{
    private readonly SchoolValidator _validator;

    public SchoolValidatorTests()
    {
        _validator = new SchoolValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_no_name(string name)
    {
        var model = new School { EstablishmentName = name };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EstablishmentName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(9998)]
    public void With_invalid_urn(int id)
    {
        var model = new School { Urn = id };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Urn);
    }

    [Theory]
    [InlineData(1000)]
    public void With_valid_urn(int id)
    {
        var model = new School { LocalAuthorityDistrictId = id };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.LocalAuthorityDistrictId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_no_address(string address)
    {
        var model = new School { Address = address };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Address);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(12)]
    [InlineData(-1)]
    public void With_invalid_establishmentTypeGroupId(int id)
    {
        var model = new School { EstablishmentTypeGroupId = id };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EstablishmentTypeGroupId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    public void With_valid_establishment_type_group_id(int id)
    {
        var model = new School { EstablishmentTypeGroupId = id };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.EstablishmentTypeGroupId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void With_valid_establishment_status_id(int id)
    {
        var model = new School { EstablishmentStatusId = id };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.EstablishmentStatusId);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public void With_invalid_establishment_status_id(int id)
    {
        var model = new School { EstablishmentStatusId = id };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EstablishmentStatusId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(9999)]
    public void With_valid_phase_of_education_id(int id)
    {
        var model = new School { PhaseOfEducationId = id };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.PhaseOfEducationId);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(8)]
    public void With_invalid_phase_of_education_id(int id)
    {
        var model = new School { PhaseOfEducationId = id };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PhaseOfEducationId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void With_invalid_local_authority_id(int id)
    {
        var model = new School { LocalAuthorityId = id };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LocalAuthorityId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void With_valid_local_authority_id(int id)
    {
        var model = new School { LocalAuthorityId = id };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.LocalAuthorityId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void With_invalid_local_authority_district_id(int id)
    {
        var model = new School { LocalAuthorityDistrictId = id };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LocalAuthorityDistrictId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void With_valid_local_authority_district_id(int id)
    {
        var model = new School { LocalAuthorityDistrictId = id };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.LocalAuthorityDistrictId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_invalid_postcode(string postcode)
    {
        var model = new School { Postcode = postcode };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Postcode);
    }
}