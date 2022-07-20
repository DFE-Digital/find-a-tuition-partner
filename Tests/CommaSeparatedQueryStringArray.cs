using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using UI.Routing;

namespace Tests;

public class CommaSeparatedQueryStringArray
{
    [Fact]
    public void Missing_key_is_not_found()
    {
        var query = new QueryCollection();

        var sut = new SeparatedQueryStringValueProvider(query, ",");

        sut.GetValue("Postcode").Should().BeEmpty();
    }

    [Fact]
    public void Scalar_value_is_unchanged()
    {
        var query = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "KeyStage", new StringValues("KeyStage1") }
        });

        var sut = new SeparatedQueryStringValueProvider(query, ",");

        sut.GetValue("KeyStage").Should().BeEquivalentTo("KeyStage1");
    }

    [Fact]
    public void Array_values_are_unchanged()
    {
        var query = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "KeyStage", new StringValues(new[]{"KeyStage1", "KeyStage2" }) }
        });

        var sut = new SeparatedQueryStringValueProvider(query, ",");

        sut.GetValue("KeyStage").Should().BeEquivalentTo(new[] { "KeyStage1", "KeyStage2" });
    }

    [Fact]
    public void Comma_separated_values_are_split()
    {
        var query = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "KeyStage", new StringValues("KeyStage1,KeyStage2") }
        });

        var sut = new SeparatedQueryStringValueProvider(query, ",");

        sut.GetValue("KeyStage").Should().BeEquivalentTo(new[] { "KeyStage1", "KeyStage2" });
    }

    [Fact]
    public void Mixture_of_array_and_separated_values_are_split()
    {
        var query = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "KeyStage", new StringValues(new[]{ "KeyStage1,KeyStage2", "KeyStage3" }) }
        });

        var sut = new SeparatedQueryStringValueProvider(query, ",");

        sut.GetValue("KeyStage").Should().BeEquivalentTo(new[] { "KeyStage1", "KeyStage2", "KeyStage3" });
    }
}
