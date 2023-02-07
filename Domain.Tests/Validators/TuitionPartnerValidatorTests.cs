using System.Collections.Generic;
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
    [InlineData("Tuition Partner")]
    public void With_valid_name(string name)
    {
        var model = new TuitionPartner { Name = name };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
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

    [Theory]
    [InlineData("Tuition partner description")]
    public void With_valid_description(string description)
    {
        var model = new TuitionPartner { Description = description };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_no_website(string website)
    {
        var model = new TuitionPartner { Website = website };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Website);
    }

    [Theory]
    [InlineData("website")]
    [InlineData("www.something.com")]
    [InlineData("www.something.org")]
    [InlineData("something.com")]
    [InlineData("something.org")]
    [InlineData("http\\www.something.org")]
    [InlineData("https\\www.something.org")]
    [InlineData("http//www.something.org")]
    [InlineData("https//www.something.org")]
    [InlineData("http:/www.something.org")]
    [InlineData("http:www.something.org")]
    [InlineData("https:www.something.org")]
    public void With_invalid_website(string website)
    {
        var model = new TuitionPartner { Website = website };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Website);
    }

    [Theory]
    [InlineData("http://www.something.org")]
    [InlineData("https://www.something.org")]

    public void With_valid_website(string website)
    {
        var model = new TuitionPartner { Website = website };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Website);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_no_email(string email)
    {
        var model = new TuitionPartner { Email = email };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("something.co.uk")]
    [InlineData("www.something.com")]
    public void With_invalid_email(string email)
    {
        var model = new TuitionPartner { Email = email };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("something@something.co.uk")]
    [InlineData("something@something.com")]
    public void With_valid_email(string email)
    {
        var model = new TuitionPartner { Email = email };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]

    public void With_no_valid_price()
    {
        var listOfPrices = new List<Price>();
        var model = new TuitionPartner { Prices = listOfPrices };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Prices);
    }

    [Fact]

    public void With_price_but_no_valid_hourly_and_groupsize()
    {
        var price = new Price { HourlyRate = 0, GroupSize = 0 };
        var listOfPrices = new List<Price>();
        listOfPrices.Add(price);
        var model = new TuitionPartner { Prices = listOfPrices };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Prices);
    }

    [Fact]

    public void With_price_and_no_valid_hourlyRate()
    {
        var price = new Price { HourlyRate = 0, GroupSize = 1 };
        var listOfPrices = new List<Price>();
        listOfPrices.Add(price);
        var model = new TuitionPartner { Prices = listOfPrices };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Prices);
    }

    [Fact]

    public void With_price_and_no_valid_groupsize()
    {
        var price = new Price { HourlyRate = 1, GroupSize = 0 };
        var listOfPrices = new List<Price>();
        listOfPrices.Add(price);
        var model = new TuitionPartner { Prices = listOfPrices };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Prices);
    }

    [Fact]

    public void With_valid_Price()
    {
        var price = new Price { HourlyRate = 1, GroupSize = 1 };
        var listOfPrices = new List<Price>();
        listOfPrices.Add(price);
        var model = new TuitionPartner { Prices = listOfPrices };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Prices);
    }

    [Fact]

    public void With_no_coverage()
    {
        var listOfCoverage = new List<LocalAuthorityDistrictCoverage>();
        var model = new TuitionPartner { LocalAuthorityDistrictCoverage = listOfCoverage };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LocalAuthorityDistrictCoverage);
    }

    [Fact]

    public void With_no_subject()
    {
        var subjectCoverage = new List<SubjectCoverage>();
        var model = new TuitionPartner { SubjectCoverage = subjectCoverage };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.SubjectCoverage);
    }

    [Fact]

    public void With_Valid_Coverage()
    {
        var listOfCoverage = new List<LocalAuthorityDistrictCoverage>();
        var localAuthorityDistrictCoverage = new LocalAuthorityDistrictCoverage();
        listOfCoverage.Add(localAuthorityDistrictCoverage);
        var model = new TuitionPartner { LocalAuthorityDistrictCoverage = listOfCoverage };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.LocalAuthorityDistrictCoverage);
    }

    [Fact]
    public void With_valid_Subject()
    {
        var subjectCoverage = new List<SubjectCoverage>();
        var tuitionPartnerCoverage = new SubjectCoverage();
        subjectCoverage.Add(tuitionPartnerCoverage);
        var model = new TuitionPartner { SubjectCoverage = subjectCoverage };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.SubjectCoverage);
    }

    [Fact]
    public void With_no_org_type()
    {
        var model = new TuitionPartner();
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.OrganisationTypeId);
    }

    [Fact]
    public void With_valid_org_type()
    {
        var model = new TuitionPartner { OrganisationTypeId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.OrganisationTypeId);
    }
}