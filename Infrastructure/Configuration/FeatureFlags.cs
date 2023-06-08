namespace Infrastructure.Configuration;

public class FeatureFlags
{
    public const string FeatureFlagsConfigName = "FeatureFlags";

    public bool VerifyEmail { get; set; } = true;
    public bool EnquiryBuilder { get; set; } = true;
    public bool SendEmailsFromNtp { get; set; } = true;
    public bool VerifyEmail { get; set; } = true;
    public bool SendTuitionPartnerEmailsWhenEnquirerDelivered { get; set; } = true;

}