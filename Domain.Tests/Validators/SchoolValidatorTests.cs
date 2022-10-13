using System;
using Domain.Constants;
using Domain.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Domain.Tests.Validators;

public class SchoolValidatorTests
{
    private readonly SchoolValidator _validator;

    private enum ErrorNumber
    {
        CannotBeNegative = -1,
        CannotBeZero = -0,

    }

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
    [InlineData((int)ErrorNumber.CannotBeZero)]
    [InlineData((int)ErrorNumber.CannotBeNegative)]
    [InlineData(9998)]
    public void With_invalid_urn(int id)
    {
        var model = new School { Urn = id };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Urn);
    }

    [Theory]
    [InlineData(10000)]
    public void With_valid_urn(int id)
    {
        var model = new School { Urn = id };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Urn);
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
    [InlineData((int)ErrorNumber.CannotBeZero)]
    [InlineData((int)ErrorNumber.CannotBeNegative)]
    public void With_invalid_establishmentTypeGroupId(int id)
    {
        var model = new School { EstablishmentTypeGroupId = id };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EstablishmentTypeGroupId);
    }

    [Fact]
    public void With_valid_establishment_type_group_id()
    {
        foreach (int establishmentType in Enum.GetValues(typeof(EstablishmentTypeGroups)))
        {
            var model = new School { EstablishmentTypeGroupId = establishmentType };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.EstablishmentTypeGroupId);
        }

    }

    [Fact]
    public void With_valid_establishment_status_id()
    {
        foreach (int establishmentState in Enum.GetValues(typeof(EstablishmentsStatus)))
        {
            var model = new School { EstablishmentStatusId = establishmentState };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.EstablishmentStatusId);
        }
    }

    [Theory]
    [InlineData((int)ErrorNumber.CannotBeZero)]
    [InlineData((int)ErrorNumber.CannotBeNegative)]
    public void With_invalid_establishment_status_id(int id)
    {
        var model = new School { EstablishmentStatusId = id };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EstablishmentStatusId);
    }

    [Fact]
    public void With_valid_phase_of_education_id()
    {
        foreach (int phasesOfEducation in Enum.GetValues(typeof(PhasesOfEducation)))
        {
            var model = new School { PhaseOfEducationId = phasesOfEducation };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.PhaseOfEducationId);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void With_invalid_phase_of_education_id(int id)
    {
        var model = new School { PhaseOfEducationId = id };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PhaseOfEducationId);
    }

    [Theory]
    [InlineData((int)ErrorNumber.CannotBeZero)]
    [InlineData((int)ErrorNumber.CannotBeNegative)]
    public void With_invalid_local_authority_id(int id)
    {
        var model = new School { LocalAuthorityId = id };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LocalAuthorityId);
    }

    [Fact]
    public void With_valid_local_authority_id()
    {
        var model = new School { LocalAuthorityId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.LocalAuthorityId);
    }

    [Theory]
    [InlineData((int)ErrorNumber.CannotBeZero)]
    [InlineData((int)ErrorNumber.CannotBeNegative)]
    public void With_invalid_local_authority_district_id(int id)
    {
        var model = new School { LocalAuthorityDistrictId = id };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LocalAuthorityDistrictId);
    }

    [Fact]
    public void With_valid_local_authority_district_id()
    {
        var model = new School { LocalAuthorityDistrictId = 1 };
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