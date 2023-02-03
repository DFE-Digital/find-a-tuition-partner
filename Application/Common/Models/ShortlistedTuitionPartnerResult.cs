namespace Application.Common.Models;

public class ShortlistedTuitionPartnerResult
{
    public bool IsCallSuccessful { get; set; }
    public int TotalShortlistedTuitionPartners { get; set; }

    public ShortlistedTuitionPartnerResult() { }

    public ShortlistedTuitionPartnerResult(bool isCallSuccessful, int totalShortlistedTuitionPartners)
    {
        IsCallSuccessful = isCallSuccessful;
        TotalShortlistedTuitionPartners = totalShortlistedTuitionPartners;
    }
}