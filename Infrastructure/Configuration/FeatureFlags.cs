namespace Infrastructure.Configuration;

public class FeatureFlags
{
    public const string FeatureFlagsConfigName = "FeatureFlags";

    public bool ValidateEmail { get; set; } = true;
    public bool EnquiryBuilder { get; set; } = true;

}