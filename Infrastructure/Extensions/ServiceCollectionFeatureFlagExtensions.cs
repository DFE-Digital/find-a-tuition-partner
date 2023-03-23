using Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceCollectionFeatureFlagExtensions
{
    public static IServiceCollection AddFeatureFlagConfig(
        this IServiceCollection services, IConfiguration config)
    {
        services.Configure<FeatureFlags>(
            config.GetSection(FeatureFlags.FeatureFlagsConfigName));

        return services;
    }
}