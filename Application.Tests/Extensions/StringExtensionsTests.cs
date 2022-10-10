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
    [InlineData("apostrophe's", "apostrophes")]
    [InlineData("special!\"£$%^&*+=chars", "specialchars")]
    [InlineData("more[];:@#,.<>?special", "morespecial")]
    [InlineData("chars-for-aspnet-operation-(){}", "chars-for-aspnet-operation-(){}")]
    [InlineData("unicode६symbols你好", "unicode-symbols")]
    public void ToSeoUrl_ReturnsKebabCase_Without_UrlEncoded_Characters(string name, string seoName)
    {
        name.ToSeoUrl().Should().Be(seoName);
    }

    [Theory]
    [InlineData("Find/Location", "find/location")]
    [InlineData("find/location", "find/location")]
    [InlineData("FindPage/LocationSearch", "find-page/location-search")]
    [InlineData("find-page/location-search", "find-page/location-search")]
    [InlineData("Find/FindAnNTPApprovedTuitionPartner", "find/find-an-ntp-approved-tuition-partner")]
    [InlineData("Find/FindATuitionPartner", "find/find-a-tuition-partner")]
    [InlineData("Find/FindATuitionPartner ", "find/find-a-tuition-partner")]
    [InlineData("Find/findATuitionPartner", "find/find-a-tuition-partner")]
    [InlineData("tuition-partner/A Tuition Co", "tuition-partner/a-tuition-co")]
    public void ToSeoUrl_ReturnsKebabCase_WhenDirectory(string camel, string kebab)
    {
        camel.ToSeoUrl().Should().Be(kebab);
    }

    [Theory]
    [InlineData("KeyStage1-Science", "key-stage-1-science")]
    [InlineData("KeyStage2 Literature", "key-stage-2-literature")]
    [InlineData("KeyStage3 Modern Foreign Languages", "key-stage-3-modern-foreign-languages")]
    public void With_spaces(string value, string expected)
    {
        value.ToSeoUrl().Should().Be(expected);
    }

    [Theory]
    [InlineData("KeyStage1", "key-stage-1")]
    public void With_trailing_digit(string value, string expected)
    {
        value.ToSeoUrl().Should().Be(expected);
    }
}