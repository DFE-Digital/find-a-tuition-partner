using Domain.Constants;
using static UI.Pages.TuitionPartner;

namespace UI.Extensions;

public static class ToStringExtensions
{
    public static bool ContainsInSchoolPrice(this Dictionary<int, GroupPrice> prices)
        => prices.Any(x => x.Value.SchoolMin.HasValue || x.Value.SchoolMax.HasValue);

    public static bool ContainsOnlinePrice(this Dictionary<int, GroupPrice> prices)
        => prices.Any(x => x.Value.OnlineMin.HasValue || x.Value.OnlineMax.HasValue);

    public static string FormatFor(this GroupPrice price, TuitionTypes tuitionType)
    {
        return tuitionType switch
        {
            TuitionTypes.InSchool => FormatPrices(price.SchoolMin, price.SchoolMax),
            TuitionTypes.Online => FormatPrices(price.OnlineMin, price.OnlineMax),
            _ => "",
        };

        static string FormatPrices(decimal? min, decimal? max) =>
            (min, max) switch
            {
                _ when min != null && min == max => $"{FormatPrice(min.Value)}",
                _ when min != null && max != null => $"{FormatPrice(min.Value)} to {FormatPrice(max.Value)}",
                _ => "",
            };
    }

    public static string FormatPrice(this decimal price)
    {
        var str = $"{price:C}";
        return str.EndsWith(".00") ? str[0..^3] : str;
    }
}