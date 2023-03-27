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

    public static string ToYesNoString(this bool value)
            => value ? "Yes" : "No";

    public static string[] SplitByLineBreaks(this string value)
        => value.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

    public static bool TryParse<TEnum>(this string displayName, out TEnum resultInputType)
        where TEnum : struct, Enum
    {
        foreach (TEnum enumLoop in Enum.GetValues(typeof(TEnum)))
        {
            var displayNameLoop = enumLoop.DisplayName();
            if (displayNameLoop.Equals(displayName, StringComparison.InvariantCultureIgnoreCase))
            {
                resultInputType = enumLoop;
                return true;
            }
        }
        resultInputType = default;
        return false;
    }
    private static string RegexReplace(this string value, string pattern, string replacement)
        => Regex.Replace(
            value, pattern, replacement,
            RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));

    public static string DisplayList(this IEnumerable<string>? strings)
    {
        if (strings == null) return string.Empty;

        var commaSeparated = string.Join(", ", strings);

        var lastCommaIndex = commaSeparated.LastIndexOf(",", StringComparison.Ordinal);

        if (lastCommaIndex == -1) return commaSeparated;

        return commaSeparated.Remove(lastCommaIndex, 1).Insert(lastCommaIndex, " and");
    }

    public static List<string> GroupByKeyAndConcatenateValues(this IEnumerable<string> keyValuePairs)
    {
        var groupedPairs = keyValuePairs.GroupBy(kv =>
            kv.Substring(0, kv.IndexOf(":", StringComparison.Ordinal)).Trim());
        return (from @group in groupedPairs
                let key = @group.Key
                let values = @group.Select(kv => kv.Substring(kv.IndexOf(":", StringComparison.Ordinal) + 1).Trim())
                    .ToList()
                let valuesCount = values.Count
                let valueString = valuesCount switch
                {
                    1 => values[0],
                    2 => $"{values[0]} and {values[1]}",
                    _ => $"{string.Join(", ", values.Take(valuesCount - 1))} and {values.Last()}"
                }
                select $"{key}: {valueString}").ToList();
    }

    public static int GetGovNotifyStatusCodeFromExceptionMessage(this string errorMessage)
    {
        var regex = new Regex(@"Status code (\d+)");
        var match = regex.Match(errorMessage);

        if (match.Success)
        {
            return int.Parse(match.Groups[1].Value);
        }

        return -1; // Return an invalid status code if no match is found
    }

    public static string? EscapeNotifyText(this string? text, bool updateForInsetFormat = false)
    {
        //Any notify special characters that are at the start of a new line need to be escaped by prefixing with \
        //The notify special characters are ^ (inset), * (bullet), # (titkes), ---- (horizontal line)
        var escapedText = text;
        if (!string.IsNullOrWhiteSpace(escapedText))
        {
            var pattern = @"^(\*|\^|\---|\#).*$";

            escapedText = Regex.Replace(escapedText, pattern, match => "\\" + match.Value, RegexOptions.Multiline);

            if (updateForInsetFormat)
            {
                //In notify if there are multiple lines it breaks the inset layout, so we add the inset ^ at the start of each line to ensure it is maintained in the email
                escapedText = escapedText.Replace(Environment.NewLine, Environment.NewLine + "^");

                //If they had started with ^ then we need to replace that
                escapedText = escapedText.Replace(Environment.NewLine + "^\\^", Environment.NewLine + "\\^");
            }
        }
        return escapedText;
    }

}