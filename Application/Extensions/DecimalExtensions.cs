namespace Application.Extensions;

public static class DecimalExtensions
{
    public static string FormatPrice(this decimal price)
    {
        var str = $"{price:C}";
        return str.EndsWith(".00") ? str[0..^3] : str;
    }
}
