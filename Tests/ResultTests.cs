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

    [Fact]
    public void ExceptionToString()
        => new ExceptionResult<Exception>(new Exception("exceptional bob")).ToString()
        .Should().Be(new Exception("exceptional bob").ToString());

    [Fact]
    public void SuccessStatusToString()
        => new SuccessStatusResult<string>("successful bob").ToString()
        .Should().Be("successful bob").ToString();

    [Fact]
    public void SuccessStatusNullToString()
        => new SuccessStatusResult<string>(null).ToString()
        .Should().Be("Success<String>").ToString();

    [Fact]
    public void ExceptionStatusNullToString()
        => new ExceptionStatusResult<string>("failed status", new Exception("exceptional alice")).ToString()
        .Should().Be("failed status (System.Exception: exceptional alice)").ToString();
}