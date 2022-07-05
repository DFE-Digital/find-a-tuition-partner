namespace Application.Extensions;

public static class SpreadsheetExtensions
{
    public static bool ParseBoolean(this string cellValue)
    {
        return cellValue.StartsWith("y", StringComparison.CurrentCultureIgnoreCase);
    }
}