using Application.Common.Structs;

namespace Application.Common.Models;
public record TuitionPartnerModel(
        string Id, string Name, bool HasLogo, string Description, string[] Subjects,
        string[] TuitionTypes, string[] Ratios, Dictionary<int, GroupPrice> Prices,
        string Website, string PhoneNumber, string EmailAddress, string[] Address, bool IsVatCharged,
        LocalAuthorityDistrictCoverage[] LocalAuthorityDistricts,
        Dictionary<Domain.Enums.TuitionType, Dictionary<Domain.Enums.KeyStage, Dictionary<string, Dictionary<int, decimal>>>> AllPrices,
        string OrganisationTypeName, string? LocalAuthorityName)
{
    public bool HasPricingVariation => Prices.Any(x => x.Value.HasVariation);
}