using Application.Common.Structs;

namespace UI.Extensions
{
    public static class GroupPriceExtensions
    {
        public static bool ContainsInSchoolPrice(this Dictionary<int, GroupPrice> prices)
            => prices.Any(x => x.Value.SchoolMin.HasValue || x.Value.SchoolMax.HasValue);

        public static bool ContainsOnlinePrice(this Dictionary<int, GroupPrice> prices)
            => prices.Any(x => x.Value.OnlineMin.HasValue || x.Value.OnlineMax.HasValue);

        public static string FormatFor(this GroupPrice price, Domain.Enums.TuitionType tuitionType)
        {
            return tuitionType switch
            {
                Domain.Enums.TuitionType.InSchool => FormatPrices(price.SchoolMin, price.SchoolMax),
                Domain.Enums.TuitionType.Online => FormatPrices(price.OnlineMin, price.OnlineMax),
                _ => "",
            };

            static string FormatPrices(decimal? min, decimal? max) =>
                (min, max) switch
                {
                    _ when min != null && min == max => $"{min.Value.FormatPrice()}",
                    _ when min != null && max != null => $"{min.Value.FormatPrice()} to {max.Value.FormatPrice()}",
                    _ => "",
                };
        }
    }
}
