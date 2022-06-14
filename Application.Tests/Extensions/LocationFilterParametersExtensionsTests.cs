using Application.Exceptions;
using Application.Extensions;
using Domain.Constants;
using Domain.Search;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Extensions;

public class LocationFilterParametersExtensionsTests
{
    [Fact]
    public void Validate_ThrowsLocationNotFoundException_WhenParametersNull()
    {
        LocationFilterParameters? parameters = null;

        var action = () => parameters.Validate();

        action.Should().Throw<LocationNotFoundException>();
    }

    [Theory]
    [InlineData(Country.Name.Wales)]
    [InlineData(Country.Name.Scotland)]
    [InlineData(Country.Name.NorthernIreland)]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ThrowsLocationNotAvailableException_WhenCountryNotEngland(string country)
    {
        var parameters = GetLocationFilterParametersForPostcode("TS1 1ON");
        parameters.Country = country;

        var action = () => parameters.Validate();

        action.Should().Throw<LocationNotAvailableException>();
    }

    [Fact]
    public void Validate_ThrowsLocationNotMappedException_WhenLocalAuthorityDistrictIsEmpty()
    {
        var parameters = GetLocationFilterParametersForPostcode("TS1 1ON");
        parameters.LocalAuthorityDistrictCode = "";

        var action = () => parameters.Validate();

        action.Should().Throw<LocationNotMappedException>();
    }

    private LocationFilterParameters GetLocationFilterParametersForPostcode(string postcode)
    {
        return new LocationFilterParameters
        {
            Postcode = postcode,
            Country = Country.Name.England,
            LocalAuthorityDistrictCode = "TEST"
        };
    }
}