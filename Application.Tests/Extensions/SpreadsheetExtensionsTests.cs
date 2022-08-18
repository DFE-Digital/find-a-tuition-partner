using System;
using Application.Extensions;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Extensions;

public class SpreadsheetExtensionsTests
{
    [Theory]
    [InlineData("Yes")]
    [InlineData("yes")]
    [InlineData("Y")]
    [InlineData("y")]
    [InlineData("1")]
    public void With_true_like(string value)
    {
        value.ParseBoolean().Should().BeTrue();
    }

    [Theory]
    [InlineData("No")]
    [InlineData("no")]
    [InlineData("N")]
    [InlineData("n")]
    [InlineData("0")]
    [InlineData("")]
    [InlineData(null)]
    public void With_false_like(string? value)
    {
        value.ParseBoolean().Should().BeFalse();
    }

    [Theory]
    [InlineData("0", 0)]
    [InlineData("0.0", 0)]
    [InlineData("-10.1", -10.1)]
    [InlineData("20.12", 20.12)]
    [InlineData("40.8", 40.8)]
    [InlineData("18.16", 18.16)]
    [InlineData("£17.67", 17.67)]
    [InlineData("12,999", 12999)]
    [InlineData("£12,999", 12999)]
    public void With_valid_decimal(string value, decimal expected)
    {
        value.ParseDecimal().Should().Be(expected);
    }

    [Theory]
    [InlineData("18.161", 18.16)]
    [InlineData("18.169", 18.17)]
    [InlineData("£17.6767", 17.68)]
    [InlineData("12,999", 12999)]
    [InlineData("15.408000000000001", 15.41)]
    [InlineData("15.407999999999999", 15.41)]
    public void Parse_price_rounds_to_two_decimal_places(string value, decimal expected)
    {
        value.ParsePrice().Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("zero")]
    [InlineData("Pi")]
    public void With_invalid_decimal(string value)
    {
        value.ParseDecimal().Should().Be(0);
    }

    [Fact]
    public void With_valid_date()
    {
        "44658.0".ParseDateOnly().Should().Be(new DateOnly(2022, 4, 7));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("Today")]
    public void With_invalid_date(string value)
    {
        value.ParseDateOnly().Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Parse_url_with_null_or_whitespace(string? url)
    {
        url.ParseUrl().Should().Be("");
    }

    [Theory]
    [InlineData("www.tuitionpartner.testdata")]
    [InlineData("tuitionpartner.testdata")]
    public void Parse_url_with_no_http_protocol(string url)
    {
        url.ParseUrl().Should().StartWith("http://");
    }

    [Theory]
    [InlineData("http://www.tuitionpartner.testdata")]
    [InlineData("https://tuitionpartner.testdata")]
    public void Parse_url_with_http_protocol(string url)
    {
        url.ParseUrl().Should().Be(url);
    }
}