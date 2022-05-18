using System.Text.Json;
using Application;
using Application.Repositories;
using Infrastructure.Configuration.GPaaS;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNtpDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NtpDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetNtpConnectionString(), action => action.MigrationsAssembly(typeof(AssemblyReference).Assembly.FullName));
        });

        services.AddScoped<INtpDbContext>(provider => provider.GetService<NtpDbContext>()!);

        return services;
    }

    public static string GetNtpConnectionString(this IConfiguration configuration)
    {
        var vcapServicesJson = configuration["VCAP_SERVICES"];
        if (!string.IsNullOrEmpty(vcapServicesJson))
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var vcapServices = JsonSerializer.Deserialize<VcapServices>(vcapServicesJson, options);
            var postgresCredentials = vcapServices?.Postgres?.FirstOrDefault()?.Credentials;

            if (postgresCredentials?.IsValid() == true)
            {
                return $"Host={postgresCredentials.Host};Port={postgresCredentials.Port};Username={postgresCredentials.Username};Password={postgresCredentials.Password};Database={postgresCredentials.Name}";
            }
        }

        return configuration.GetConnectionString("NtpDatabase");
    }

    public static IServiceCollection AddSearchRequestBuilder(this IServiceCollection services)
    {
        services.AddScoped<ISearchStateRepository, InMemorySearchStateRepository>();
        services.AddScoped<ISearchRequestBuilderRepository, SearchRequestBuilderRepository>();

        services.AddHttpClient<ILocationFilterService, PostcodesIoLocationFilterService>(client =>
        {
            client.BaseAddress = new Uri("https://api.postcodes.io");
        });

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ILookupDataRepository, LookupDataRepository>();

        return services;
    }
}