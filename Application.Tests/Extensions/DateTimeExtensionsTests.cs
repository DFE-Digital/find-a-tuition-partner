using System;
using Application.Extensions;
using Xunit;

namespace Application.Tests.Extensions;

public class DateTimeExtensionsTests
{
    [Fact]
    public void ToLocalDateTime_ShouldConvertToCorrectTimeZone_Given_UtcDateTime()
    {
        // Arrange
        var dateTimeUtc = new DateTime(2023, 3, 29, 12, 0, 0, DateTimeKind.Utc);
        var timeZoneId = "GMT Standard Time";
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var expectedDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, timeZoneInfo);

        // Act
        var result = dateTimeUtc.ToLocalDateTime(timeZoneId);

        // Assert
        Assert.Equal(expectedDateTime, result);
    }
}