using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Application.Constants;

namespace Application.Extensions;

public static class StringExtensions
{
    [return: NotNullIfNotNull("value")]
    public static string? ToSeoUrl(this string? value)
    {
        var seo = value?
            .RegexReplace(StringConstants.CamelCaseBoundaries, " $1")
            .Trim()
            .RegexReplace(StringConstants.SpacesAndUnderscore, "-")
            .RegexReplace(StringConstants.UrlUnsafeCharacters, "")
            .RegexReplace(StringConstants.MultipleUnderscores, "-")
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