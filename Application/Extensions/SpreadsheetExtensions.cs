using System.Text.RegularExpressions;

namespace Application.Extensions;

public static class SpreadsheetExtensions
{
    public static bool ParseBoolean(this string? cellValue)
    {
        if (cellValue == null) return false;

        if (int.TryParse(cellValue, out var intValue) && intValue == 1) return true;

        return cellValue.StartsWith("y", StringComparison.CurrentCultureIgnoreCase);
    }

    public static decimal ParseDecimal(this string? cellValue)
    {
        if (cellValue == null) return 0;

        cellValue = cellValue.Replace("£", "");

        if (decimal.TryParse(cellValue, out var decimalValue)) return decimalValue;

        return 0;
    }

    public static DateOnly? ParseDateOnly(this string? cellValue)
    {
        if (double.TryParse(cellValue, out var doubleValue))
        {
            return DateOnly.FromDateTime(DateTime.FromOADate(doubleValue));
        }

        return null;
    }

    public static string ParseUrl(this string? cellValue)
    {
        if (string.IsNullOrWhiteSpace(cellValue)) return "";

        if (Regex.IsMatch(cellValue, @"^https?://", RegexOptions.IgnoreCase)) return cellValue;

        return $"http://{cellValue}";
    }
}