using Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceCollectionServiceUnavailableSettingsExtensions
{
    public static IServiceCollection AddServiceUnavailableSettingsConfig(
        this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ServiceUnavailableSettings>(
            config.GetSection(ServiceUnavailableSettings.ServiceUnavailableSettingsConfigName));

        return services;
    }

    public static bool IsServiceUnavailable(this ServiceUnavailableSettings serviceUnavailableSettings)
    {
        return (serviceUnavailableSettings.StartDateTime == null ||
            serviceUnavailableSettings.StartDateTime < DateTime.Now.ToLocalTime()) &&
            serviceUnavailableSettings.EndDateTime != null &&
            serviceUnavailableSettings.EndDateTime > DateTime.Now.ToLocalTime();
    }
}