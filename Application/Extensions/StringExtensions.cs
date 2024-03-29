using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Web;
using Application.Common.Models.Enquiry;
using Application.Constants;
using Domain.Enums;
using Newtonsoft.Json;

namespace Application.Extensions;

public static class StringExtensions
{
    [return: NotNullIfNotNull("value")]
    public static string? ToSeoUrl(this string? value, bool replaceForwardSlash = true)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var seo = value;

        if (replaceForwardSlash)
            seo = seo.Replace("/", "");

        seo = seo
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

    public static bool TryParseTribalTuitionSetting(this string tuitionSettingString, out TuitionSetting tuitionSetting)
    {
        var tuitionSettingStringReplaced = tuitionSettingString.Equals("Both", StringComparison.InvariantCultureIgnoreCase) ? TuitionSetting.NoPreference.DisplayName() : tuitionSettingString;
        tuitionSettingStringReplaced = tuitionSettingStringReplaced.Replace("In School", TuitionSetting.FaceToFace.DisplayName(), StringComparison.InvariantCultureIgnoreCase);
        tuitionSettingStringReplaced = tuitionSettingStringReplaced.Replace(TuitionSetting.FaceToFace.DisplayName().Replace("-", " "), TuitionSetting.FaceToFace.DisplayName(), StringComparison.InvariantCultureIgnoreCase);
        return tuitionSettingStringReplaced.TryParse(out tuitionSetting);
    }

    public static bool TryParse<TEnum>(this string displayName, out TEnum resultInputType)
        where TEnum : struct, Enum
    {
        foreach (TEnum enumLoop in Enum.GetValues(typeof(TEnum)))
        {
            if (enumLoop.DisplayName().Equals(displayName, StringComparison.InvariantCultureIgnoreCase))
            {
                resultInputType = enumLoop;
                return true;
            }
        }
        resultInputType = default;
        return false;
    }

    public static TEnum GetEnumFromDisplayName<TEnum>(this string displayName)
        where TEnum : struct, Enum
    {
        var enumTryParse = displayName.TryParse(out TEnum returnEnum);

        if (!enumTryParse)
            throw new ArgumentException($"Invalid enum display name.  Enum {typeof(TEnum)} not got matching display name {displayName}");

        return returnEnum;
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
        //The notify special characters are ^ (inset), * (bullet), # (titles), ---- (horizontal line)
        var escapedText = text;
        if (!string.IsNullOrWhiteSpace(escapedText))
        {
            //Match on first character on line, ignore white space and tabs at start of line
            var pattern = @"^[ \t]*(\*|\^|\---|\#).*$";

            //We can trim here, since Notify trims the content anyway
            escapedText = Regex.Replace(escapedText, pattern, match => "\\" + match.Value.TrimStart(new char[] { ' ', '\t' }), RegexOptions.Multiline);

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

    public static string CreateNotifyEnquiryClientReference(this string enquiryRef, string clientRefPrefix, EmailTemplateType emailTemplateType, string? tpName = null)
    {
        var tpSeoUrl = string.IsNullOrWhiteSpace(tpName) ? string.Empty : $"-{tpName.ToSeoUrl()}";
        clientRefPrefix = string.IsNullOrWhiteSpace(clientRefPrefix) ? string.Empty : $"{clientRefPrefix.ToSeoUrl()}-";
        return $"{clientRefPrefix}{enquiryRef}-{emailTemplateType.DisplayName()}{tpSeoUrl}";
    }

    public static string CreateNotifyEmailClientReference(this string emailTemplateTypeDisplayName, string clientRefPrefix)
    {
        clientRefPrefix = string.IsNullOrWhiteSpace(clientRefPrefix) ? string.Empty : $"{clientRefPrefix.ToSeoUrl()}-";
        return $"{clientRefPrefix}{emailTemplateTypeDisplayName}";
    }

    public static string ExtractFileNameFromDirectory(this string path)
    {
        var regex = new Regex(@"[^\\/]+$");
        var match = regex.Match(path);
        return match.Value;
    }

    public static TutoringLogisticsDetailsModel? ToTutoringLogisticsDetailsModel(this string tutoringLogisticsDetailsModelJson)
    {
        if (string.IsNullOrEmpty(tutoringLogisticsDetailsModelJson))
            return null;

        try
        {
            return JsonConvert.DeserializeObject<TutoringLogisticsDetailsModel>(tutoringLogisticsDetailsModelJson);
        }
        catch
        {
            return null;
        }
    }

    public static string? ToSanitisedPostcode(this string? postcode)
    {
        if (string.IsNullOrEmpty(postcode))
            return postcode;

        var updatedPostcode = ToSanitisedPostcodeInner(postcode!);

        if (string.IsNullOrEmpty(updatedPostcode))
        {
            updatedPostcode = ToSanitisedPostcodeInner(HttpUtility.UrlDecode(postcode!));
        }

        return updatedPostcode;
    }

    private static string ToSanitisedPostcodeInner(this string postcode)
    {
        var updatedPostcode = Regex.Replace(postcode, "[^A-Za-z0-9]", "").ToUpper();

        var regex = new Regex(StringConstants.PostcodeRegExp);
        var match = regex.Match(updatedPostcode);

        if (!match.Success)
            return string.Empty;

        return $"{updatedPostcode.Substring(0, updatedPostcode.Length - 3)} {updatedPostcode.Substring(updatedPostcode.Length - 3, 3)}";
    }
}