using Application.Common.Interfaces;
using Infrastructure.Configuration;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notify.Client;
using Notify.Interfaces;

namespace Infrastructure.Extensions;

public static class ServiceCollectionNotificationClientExtensions
{
    public static IServiceCollection AddNotificationConfig(
        this IServiceCollection services, IConfiguration config)
    {
        services.Configure<GovUkNotifyOptions>(
            config.GetSection(GovUkNotifyOptions.GovUkNotify));

        return services;
    }
    public static IServiceCollection AddNotificationClientServiceConfiguration(this IServiceCollection services, IConfiguration config)
    {
        var govUkNotifyConfig = config.GetSection(GovUkNotifyOptions.GovUkNotify).Get<GovUkNotifyOptions>();
        if (!string.IsNullOrEmpty(govUkNotifyConfig?.ApiKey))
        {
            services.AddScoped<IAsyncNotificationClient, NotificationClient>(
                _ => new NotificationClient(govUkNotifyConfig?.ApiKey));
        }

        services.AddOptions();
        services.AddScoped<INotificationsClientService, NotificationsClientService>();
        return services;
    }
}