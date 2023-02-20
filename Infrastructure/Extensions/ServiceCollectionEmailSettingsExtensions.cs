using Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceCollectionEmailSettingsExtensions
{
    public static IServiceCollection AddEmailSettingsConfig(
        this IServiceCollection services, IConfiguration config)
    {
        services.Configure<EmailSettings>(
            config.GetSection(EmailSettings.EmailSettingsConfigName));

        return services;
    }
}