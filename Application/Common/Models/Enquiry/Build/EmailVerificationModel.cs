namespace Application.Common.Models.Enquiry.Build;

public record EmailVerificationModel : EnquiryBuildModel
{
    public string? Passcode { get; set; }
    public string? PasscodeForTesting { get; set; }

    public string? NewPasscodeSentAt { get; set; }
}