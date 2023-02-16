using Application.Common.Models;

namespace UI.Models;

public record GetTuitionPartnerQueryModel(string Id, [FromQuery(Name = "show-locations-covered")]
    bool ShowLocationsCovered = false,
    [FromQuery(Name = "show-full-pricing")]
    bool ShowFullPricing = false,
    [FromQuery(Name = "show-full-info")]
    bool ShowFullInfo = false) : SearchModel
{
    public Dictionary<string, string> ToRouteData()
    {
        var dictionary = new Dictionary<string, string>
        {
            [nameof(Id)] = Id
        };

        if (ShowLocationsCovered) dictionary["show-locations-covered"] = "true";

        if (ShowFullPricing) dictionary["show-full-pricing"] = "true";

        if (ShowFullInfo) dictionary["show-full-info"] = "true";

        return dictionary;
    }
}