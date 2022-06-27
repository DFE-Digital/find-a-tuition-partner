using Microsoft.Extensions.DependencyInjection;

namespace Tests.Utilities;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Remove<T>(this IServiceCollection services)
    {
        var descriptor = services.Single(d => d.ServiceType == typeof(T));
        services.Remove(descriptor);
        return services;
    }

    public static T? CreateWithDependenciesFromServices<T>(this IServiceProvider services) where T : class
    {
        var constructors = typeof(T).GetConstructors();

        var parameterInfo = constructors.First().GetParameters();

        var parameters = parameterInfo.Select(p => services.GetRequiredService(p.ParameterType)).ToArray();

        var page = Activator.CreateInstance(typeof(T), parameters) as T;

        return page;
    }
}