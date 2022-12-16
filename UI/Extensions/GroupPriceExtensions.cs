namespace UI.Extensions
{
    public static class GroupPriceExtensions
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
                    _ when min != null && min == max => $"{min.Value.FormatPrice()}",
                    _ when min != null && max != null => $"{min.Value.FormatPrice()} to {max.Value.FormatPrice()}",
                    _ => "",
                };
        }
    }
}
