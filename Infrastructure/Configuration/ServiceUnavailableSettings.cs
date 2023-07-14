namespace Infrastructure.Configuration;

public class ServiceUnavailableSettings
{
    public const string ServiceUnavailableSettingsConfigName = "ServiceUnavailableSettings";

    public string Message { get; set; } = "You will be able to use the service from {EndDateTime}.";
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }

}