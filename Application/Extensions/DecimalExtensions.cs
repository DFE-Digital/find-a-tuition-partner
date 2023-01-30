namespace Application.Extensions;

public static class DecimalExtensions
{
    public static string FormatPrice(this decimal price, bool? removeVAT = false)
    {
        var adjustedPrice = price;
        if (removeVAT ?? false)
        {
            adjustedPrice = ((price / 120) * 100);
            adjustedPrice = Math.Round(adjustedPrice, 2);
        }
        var str = $"{adjustedPrice:C}";
        return str.EndsWith(".00") ? str[0..^3] : str;
    }
}
