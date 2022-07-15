using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Application.Extensions;

public static class StringExtensions
{
    [return: NotNullIfNotNull("value")]
    public static string? ToSeoUrl(this string? value)
    {
        if (value == null) return null;

        var withSpaces = Regex.Replace(value, @"((?<=[a-z])[A-Z]|(?<=[^\-\W])[A-Z](?=[a-z])|(?<=[a-z])\d+)", " $1", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100)).Trim();

        var spacesToDash = withSpaces.Replace(' ', '-');

        return spacesToDash.ToLower();
    }
}