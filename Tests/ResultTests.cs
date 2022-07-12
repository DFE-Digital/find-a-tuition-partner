using Domain;

namespace Tests;

public class ResultToStringTests
{
    [Fact]
    public void SuccessToString() => new SuccessResult().ToString().Should().Be("Success");

    [Fact]
    public void SuccessOfStringToString()
        => new SuccessResult<string>("bob").ToString().Should().Be("bob");

    [Fact]
    public void SuccessOfIntToString()
        => new SuccessResult<int>(12).ToString().Should().Be("12");

    [Fact]
    public void ErrorToString()
        => new ErrorResult().ToString().Should().Be("Error");

    [Fact]
    public void ErrorOfTToString()
        => new ErrorResult<string, string>("bob").ToString().Should().Be("bob");
}