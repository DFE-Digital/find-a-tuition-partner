using FluentAssertions;
using Infrastructure.Services;
using Xunit;

namespace Infrastructure.Tests.Services;

public class RandomTokenGeneratorServiceTests
{
    private readonly RandomTokenGeneratorService _sut;

    public RandomTokenGeneratorServiceTests()
    {
        _sut = new RandomTokenGeneratorService();
    }

    [Fact]
    public void GenerateRandomToken_ReturnsUniqueUrlSafeTokens()
    {
        // Act
        var token1 = _sut.GenerateRandomToken();
        var token2 = _sut.GenerateRandomToken();

        // Assert
        token1.Should().NotBeNullOrEmpty();
        token2.Should().NotBeNullOrEmpty();
        token1.Should().NotBe(token2);

        // Check that tokens are URL-safe
        token1.Should().MatchRegex("^[A-Za-z0-9-_]+$");
        token2.Should().MatchRegex("^[A-Za-z0-9-_]+$");

        // Check that tokens have the correct length including padding characters (=) are being added to the end of the token.
        Assert.InRange(token1.Length, 32, 34);
        Assert.InRange(token2.Length, 32, 34);
    }
}