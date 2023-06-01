namespace Application.Common.Models.Enquiry.Build;

public record EmailVerificationModel : EnquiryBuildModel
{
    public int? Passcode { get; set; }
    public int? PasscodeForTesting { get; set; }
}