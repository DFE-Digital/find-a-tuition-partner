using Application.Common.Structs;
using KeyStage = Domain.Enums.KeyStage;
using TuitionSetting = Domain.Enums.TuitionSetting;

namespace Application.Common.Models;
public record TuitionPartnerModel(
        string Id, string Name, bool HasLogo, string Description, string[] Subjects,
        string[] TuitionSettings, string[] Ratios, Dictionary<int, GroupPrice> Prices,
        string Website, string PhoneNumber, string EmailAddress, string[] Address, bool IsVatCharged,
        LocalAuthorityDistrictCoverage[] LocalAuthorityDistricts,
        Dictionary<TuitionSetting, Dictionary<KeyStage, Dictionary<string, Dictionary<int, decimal>>>> AllPrices,
        string OrganisationTypeName, string? LocalAuthorityDistrictName, DateTime? TPLastUpdatedData,
        DateTime? ImportProcessLastUpdatedData, string? ImportId, bool? IsActive)
{
    public bool HasPricingVariation => Prices.Any(x => x.Value.HasVariation);
}