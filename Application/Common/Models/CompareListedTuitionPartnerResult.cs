namespace Application.Common.Models;

public class CompareListedTuitionPartnerResult
{
    public bool IsCallSuccessful { get; set; }
    public int TotalCompareListedTuitionPartners { get; set; }

    public CompareListedTuitionPartnerResult() { }

    public CompareListedTuitionPartnerResult(bool isCallSuccessful, int totalCompareListedTuitionPartners)
    {
        IsCallSuccessful = isCallSuccessful;
        TotalCompareListedTuitionPartners = totalCompareListedTuitionPartners;
    }
}