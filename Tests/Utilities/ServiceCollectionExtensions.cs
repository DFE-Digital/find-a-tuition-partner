using Microsoft.Extensions.DependencyInjection;

namespace Tests.Utilities;

public static class ServiceCollectionExtensions
{
    public static T? CreateWithDependenciesFromServices<T>(this IServiceProvider services) where T : class
    {
        var constructors = typeof(T).GetConstructors();

        var parameterInfo = constructors.First().GetParameters();

        var parameters = parameterInfo.Select(p => services.GetRequiredService(p.ParameterType));

        var page = Activator.CreateInstance(typeof(T), parameters) as T;

        return page;
    }
}