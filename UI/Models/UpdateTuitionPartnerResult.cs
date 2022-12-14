namespace UI.Models;

public class UpdateTuitionPartnerResult
{
    public bool IsCallSuccessful { get; set; }
    public int TotalShortlistedTuitionPartners { get; set; }

    public UpdateTuitionPartnerResult() { }

    public UpdateTuitionPartnerResult(bool isCallSuccessful, int totalShortlistedTuitionPartners)
    {
        IsCallSuccessful = isCallSuccessful;
        TotalShortlistedTuitionPartners = totalShortlistedTuitionPartners;
    }
}