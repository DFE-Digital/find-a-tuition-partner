using System.Text.RegularExpressions;

namespace Application.Extensions;

public static class StringExtensions
{
    public static string? ToSeoUrl(this string? value)
    {
        if (value == null) return null;

        var withSpaces = Regex.Replace(value, "((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", " $1", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100)).Trim();

        var spacesToDash = withSpaces.Replace(' ', '-');

        var withoutDashAfterSeparator = spacesToDash.Replace("/-", "/");

        return withoutDashAfterSeparator.ToLower();
    }
}