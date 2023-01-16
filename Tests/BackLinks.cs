using UI.Enums;
using UI.Extensions;
using UI.Models;

namespace Tests;

public class BackLinks
{
    [Fact]
    public void Construct_querystring_from_string_property()
    {
        var model = new SearchModel { Postcode = "AB1 2CD" };
        var result = model.ToQueryString();
        result.Should().Be("Postcode=AB1 2CD");
    }

    [Fact]
    public void Construct_querystring_from_array_property_with_one_item()
    {
        var model = new SearchModel { Subjects = new[] { "first" } };
        var result = model.ToQueryString();
        result.Should().Be("Subjects=first");
    }

    [Fact]
    public void Construct_querystring_from_array_property_with_multiple_items()
    {
        var model = new SearchModel
        {
            KeyStages = new[]
            {
                KeyStage.KeyStage1, KeyStage.KeyStage2, KeyStage.KeyStage3
            }
        };

        var result = model.ToQueryString();

        result.Should().Be("KeyStages=KeyStage1&KeyStages=KeyStage2&KeyStages=KeyStage3");
    }

    [Fact]
    public void Construct_querystring_from_array_property_with_null_item()
    {
        var model = new SearchModel { Subjects = new string[] { null! } };
        var result = model.ToQueryString();
        result.Should().BeEmpty();
    }

    [Fact]
    public void Construct_querystring_from_non_null_search_model_properties()
    {
        var model = new SearchModel
        {
            Postcode = "AB1 2CD",
            KeyStages = new[] { KeyStage.KeyStage1 },
            Subjects = null,
        };

        var result = model.ToQueryString();

        result.Should().Be("Postcode=AB1 2CD&KeyStages=KeyStage1");
    }

    [Fact]
    public void Construct_querystring_from_tutor_type()
    {
        var model = new SearchModel { TuitionType = TuitionType.Online };
        var result = model.ToQueryString();
        result.Should().Be("TuitionType=Online");
    }

    [Fact]
    public void Construct_querystring_from_organisation_grouping_type()
    {
        var model = new SearchModel { OrganisationTypeGrouping = OrganisationTypeGrouping.Charity };
        var result = model.ToQueryString();
        result.Should().Be("OrganisationTypeGrouping=Charity");
    }
}