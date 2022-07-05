using System.Text.Json;
using Application;
using Application.Extraction;
using Application.Factories;
using Application.Repositories;
using Infrastructure.Configuration.GPaaS;
using Infrastructure.Extraction;
using Infrastructure.Factories;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.DataProtection;
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

        services.AddDataProtection().PersistKeysToDbContext<NtpDbContext>();

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
                return $"Host={postgresCredentials.Host};Port={postgresCredentials.Port};Username={postgresCredentials.Username};Password={postgresCredentials.Password};Database={postgresCredentials.Name};Include Error Detail=true";
            }
        }

        return configuration.GetConnectionString("NtpDatabase") + ";Include Error Detail=true";
    }

    public static IServiceCollection AddLocationFilterService(this IServiceCollection services)
    {
        services.AddHttpClient<ILocationFilterService, PostcodesIoLocationFilterService>(client =>
        {
            client.BaseAddress = new Uri("https://api.postcodes.io");
        });

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGeographyLookupRepository, GeographyLookupRepository>();
        services.AddScoped<ILookupDataRepository, LookupDataRepository>();
        services.AddScoped<ITuitionPartnerRepository, TuitionPartnerRepository>();

        return services;
    }

    public static IServiceCollection AddDataImporter(this IServiceCollection services)
    {
        services.AddScoped<ISpreadsheetExtractor, OpenXmlSpreadsheetExtractor>();
        services.AddScoped<ITuitionPartnerFactory, QualityAssuredSpreadsheetTuitionPartnerFactory>();

        return services;
    }
}