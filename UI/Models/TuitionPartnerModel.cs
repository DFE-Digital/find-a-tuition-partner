namespace UI.Models;
public record TuitionPartnerModel(
        string Id, string Name, bool HasLogo, string Description, string[] Subjects,
        string[] TuitionTypes, string[] Ratios, Dictionary<int, GroupPrice> Prices,
        string Website, string PhoneNumber, string EmailAddress, string[] Address, bool HasSenProvision, bool IsVatCharged,
        LocalAuthorityDistrictCoverage[] LocalAuthorityDistricts,
        Dictionary<Enums.TuitionType, Dictionary<Enums.KeyStage, Dictionary<string, Dictionary<int, decimal>>>> AllPrices,
        string LegalStatus, string? LocalAuthorityName)
{
    public bool HasPricingVariation => Prices.Any(x => x.Value.HasVariation);
}