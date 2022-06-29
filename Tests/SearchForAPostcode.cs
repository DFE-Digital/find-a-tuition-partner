using Application.Exceptions;
using Domain.Search;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using UI.Pages.FindATuitionPartner;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class SearchForAPostcode
{
    private readonly SliceFixture _fixture;

    public SearchForAPostcode(SliceFixture fixture) => _fixture = fixture;

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void With_no_postcode(string postcode)
    {
        var model = new Location.Command { Postcode = postcode };
        var result = new Location.Validator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Postcode)
            .WithErrorMessage("Enter a postcode");
    }

    [Theory]
    [InlineData("not a postcode")]
    public void With_an_invalid_postcode(string postcode)
    {
        var model = new Location.Command { Postcode = postcode };
        var result = new Location.Validator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Postcode)
            .WithErrorMessage("Enter a valid postcode");
    }

    [Theory]
    [InlineData("AA0 0AA")]
    public async void With_a_postcode_that_is_not_found(string postcode)
    {
        _fixture.LocationFilter.GetLocationFilterParametersAsync(postcode)
            .Returns(null as LocationFilterParameters);

        await _fixture.Invoking(f => f.SendAsync(new Location.Command { Postcode = postcode }))
            .Should().ThrowAsync<LocationNotFoundException>();
    }

    [Theory]
    [InlineData("LL58 8EP", "Wales")]
    [InlineData("DD1 4NP", "Scotland")]
    public async void With_a_postcode_outside_of_England(string postcode, string country)
    {
        _fixture.LocationFilter.GetLocationFilterParametersAsync(postcode)
            .Returns(new LocationFilterParameters { Country = country });

        await _fixture.Invoking(f => f.SendAsync(new Location.Command { Postcode = postcode }))
            .Should().ThrowAsync<LocationNotAvailableException>();
    }

    [Theory]
    [InlineData("LL58 8EP")]
    [InlineData("DD1 4NP")]
    public async void With_a_postcode_that_cannot_be_mapped(string postcode)
    {
        _fixture.LocationFilter.GetLocationFilterParametersAsync(postcode)
            .Returns(new LocationFilterParameters
            {
                Country = "England",
                LocalAuthorityDistrictCode = null,
            });

        await _fixture.Invoking(f => f.SendAsync(new Location.Command { Postcode = postcode }))
            .Should().ThrowAsync<LocationNotMappedException>();
    }

    [Theory]
    [InlineData("LL58 8EP")]
    [InlineData("DD1 4NP")]
    public async void With_a_valid_postcode(string postcode)
    {
        _fixture.LocationFilter.GetLocationFilterParametersAsync(postcode)
            .Returns(new LocationFilterParameters
            {
                Country = "England",
                LocalAuthorityDistrictCode = "ebcdic",
            });

        var result = await _fixture.GetPage<Location>().Execute(page =>
        {
            page.Data.Postcode = postcode;
            return page.OnPost();
        });

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be("KeyStages");
        redirect.RouteValues.Should().ContainKey("Postcode").WhoseValue.Should().Be(postcode);
    }
}