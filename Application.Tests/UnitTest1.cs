using FluentAssertions;
using Xunit;

namespace Application.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var sut = new { Something = true };

        sut.Something.Should().Be(true);
    }
}