namespace Application.Extensions;

public static class DecimalExtensions
{
    //Current VAT is 20%
    private const decimal VATRate = 20m;

    public static string FormatPrice(this decimal price)
    {
        var str = $"{price:C}";
        return str.EndsWith(".00") ? str[0..^3] : str;
    }

    public static decimal AddVAT(this decimal price)
    {
        return Math.Round(price * ((100 + VATRate) / 100), 2);
    }

    public static decimal RemoveVAT(this decimal price)
    {
        return Math.Round(price / (100 + VATRate) * 100, 2);
    }
}
