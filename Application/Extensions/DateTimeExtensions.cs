namespace Application.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToLocalDateTime(this DateTime dateTime, string? timeZoneId = null)
    {
        if (string.IsNullOrEmpty(timeZoneId)) timeZoneId = "GMT Standard Time";
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZoneInfo);
    }
}