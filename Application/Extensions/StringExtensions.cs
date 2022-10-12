using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Application.Extensions;

public static class StringExtensions
{
    private const string CamelCaseBoundaries = @"((?<=[a-z])[A-Z]|(?<=[^\-\W])[A-Z](?=[a-z])|(?<=[a-z])\d+)";
    private const string UrlUnsafeCharacters = "[^a-zA-Z0-9_{}()\\-~/]";

    [return: NotNullIfNotNull("value")]
    public static string? ToSeoUrl(this string? value)
    {
        var seo = value?
            .RegexReplace(CamelCaseBoundaries, " $1")
            .Trim()
            .Replace(' ', '-')
            .RegexReplace(UrlUnsafeCharacters, "")
            .ToLower();

        return seo;
    }

    private static string RegexReplace(this string value, string pattern, string replacement)
        => Regex.Replace(
            value, pattern, replacement,
            RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));

    public static string ToYesNoString(this bool value)
            => value ? "Yes" : "No";

    public static string[] SplitByLineBreaks(this string value)
        => value.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
}