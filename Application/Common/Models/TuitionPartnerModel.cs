using Application.Common.Structs;
using KeyStage = Domain.Enums.KeyStage;
using TuitionType = Domain.Enums.TuitionType;

namespace Application.Common.Models;
public record TuitionPartnerModel(
        string Id, string Name, bool HasLogo, string Description, string[] Subjects,
        string[] TuitionTypes, string[] Ratios, Dictionary<int, GroupPrice> Prices,
        string Website, string PhoneNumber, string EmailAddress, string[] Address, bool IsVatCharged,
        LocalAuthorityDistrictCoverage[] LocalAuthorityDistricts,
        Dictionary<TuitionType, Dictionary<KeyStage, Dictionary<string, Dictionary<int, decimal>>>> AllPrices,
        string OrganisationTypeName, string? LocalAuthorityName, DateTime? TPLastUpdatedData,
        DateTime? ImportProcessLastUpdatedData, string? ImportId, bool? IsActive)
{
    public bool HasPricingVariation => Prices.Any(x => x.Value.HasVariation);
}