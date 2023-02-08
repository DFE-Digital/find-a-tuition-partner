using Application.Extensions;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Extensions;

public class DecimalExtensionsTests
{
    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(10.46, 12.55)]
    [InlineData(10.47, 12.56)]
    [InlineData(10.48, 12.58)]
    [InlineData(10.49, 12.59)]
    [InlineData(10.50, 12.60)]
    [InlineData(10.51, 12.61)]
    [InlineData(10.52, 12.62)]
    [InlineData(10.53, 12.64)]
    public void AddVAT_AddsCorrectPercentage(decimal vatExclusive, decimal vatInclusive)
    {
        vatExclusive.AddVAT().Should().Be(vatInclusive);
    }

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(12.55, 10.46)]
    [InlineData(12.56, 10.47)]
    [InlineData(12.57, 10.48)]
    [InlineData(12.58, 10.48)]
    [InlineData(12.59, 10.49)]
    [InlineData(12.60, 10.50)]
    [InlineData(12.61, 10.51)]
    [InlineData(12.62, 10.52)]
    [InlineData(12.63, 10.53)]
    [InlineData(12.64, 10.53)]
    public void RemoveVAT_RemovesCorrectPercentage(decimal vatExclusive, decimal vatInclusive)
    {
        vatExclusive.RemoveVAT().Should().Be(vatInclusive);
    }
}