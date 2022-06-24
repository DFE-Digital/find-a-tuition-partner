using Application.Extensions;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void ToSeoUrl_ReturnsNull_WhenValueNull()
    {
        string? value = null;

        value.ToSeoUrl().Should().BeNull();
    }

    [Fact]
    public void ToSeoUrl_ReturnsEmptyString_WhenValueEmptyString()
    {
        const string value = "";

        value.ToSeoUrl().Should().Be("");
    }

    [Fact]
    public void ToSeoUrl_ReturnsEmptyString_WhenValueWhitespace()
    {
        const string value = "   ";

        value.ToSeoUrl().Should().Be("");
    }

    [Fact]
    public void ToSeoUrl_ReturnsKebabCase_WhenValuePascalCase()
    {
        const string value = "FindATuitionPartner";

        value.ToSeoUrl().Should().Be("find-a-tuition-partner");
    }

    [Fact]
    public void ToSeoUrl_ReturnsTrimmedKebabCase_WhenValueContainsWhitespace()
    {
        const string value = " FindATuitionPartner ";

        value.ToSeoUrl().Should().Be("find-a-tuition-partner");
    }

    [Fact]
    public void ToSeoUrl_ReturnsKebabCaseWithoutSplitting_WhenValueContainsUppercase()
    {
        const string value = "FindAnNTPApprovedTuitionPartner";

        value.ToSeoUrl().Should().Be("find-an-ntp-approved-tuition-partner");
    }

    [Fact]
    public void ToSeoUrl_ReturnsKebabCase_WhenValueKebabCase()
    {
        const string value = "find-a-tuition-partner";

        value.ToSeoUrl().Should().Be("find-a-tuition-partner");
    }

    [Fact]
    public void ToSeoUrl_ReturnsKebabCase_WhenValueCamelCase()
    {
        const string value = "searchId";

        value.ToSeoUrl().Should().Be("search-id");
    }

    [Theory]
    [InlineData("Find/Location", "find/location")]
    [InlineData("find/location", "find/location")]
    [InlineData("FindPage/LocationSearch", "find-page/location-search")]
    [InlineData("find-page/location-search", "find-page/location-search")]
    [InlineData("Find/FindAnNTPApprovedTuitionPartner", "find/find-an-ntp-approved-tuition-partner")]
    [InlineData("Find/FindATuitionPartner", "find/find-a-tuition-partner")]
    [InlineData("Find/FindATuitionPartner ", "find/find-a-tuition-partner")]
    public void ToSeoUrl_ReturnsKebabCase_WhenDirectory(string camel, string kebab)
    {
        camel.ToSeoUrl().Should().Be(kebab);
    }
}