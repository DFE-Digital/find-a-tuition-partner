using System.Text.Json;
using Application;
using Application.Common.Interfaces;
using Application.DataImport;
using Application.Extraction;
using Application.Factories;
using Application.Repositories;
using Infrastructure.Configuration;
using Infrastructure.Configuration.GPaaS;
using Infrastructure.Constants;
using Infrastructure.DataImport;
using Infrastructure.Extraction;
using Infrastructure.Factories;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNtpDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NtpDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetNtpConnectionString(),
                action =>
                {
                    action.MigrationsAssembly(typeof(AssemblyReference).Assembly.FullName);
                    action.EnableRetryOnFailure();
                });
        });

        services.AddScoped<INtpDbContext>(provider => provider.GetService<NtpDbContext>()!);

        services.AddDataProtection().PersistKeysToDbContext<NtpDbContext>();

        return services;
    }

    public static IServiceCollection AddDistributedCache(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetRedisConnectionString();

        if (connectionString != null)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        return services;
    }

    public static VcapServices? GetVcapServices(this IConfiguration configuration)
    {
        var vcapServicesJson = configuration["VCAP_SERVICES"];

        if (string.IsNullOrEmpty(vcapServicesJson))
        {
            return null;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<VcapServices>(vcapServicesJson, options);
    }
    public static string GetNtpConnectionString(this IConfiguration configuration)
    {
        var vcapServices = configuration.GetVcapServices();

        if (vcapServices != null)
        {
            var postgresCredentials = vcapServices?.Postgres?.FirstOrDefault()?.Credentials;

            if (postgresCredentials?.IsValid() == true)
            {
                return $"Host={postgresCredentials.Host};Port={postgresCredentials.Port};Username={postgresCredentials.Username};Password={postgresCredentials.Password};Database={postgresCredentials.Name}";
            }
        }

        return configuration.GetConnectionString(EnvironmentVariables.FatpDatabaseConnectionString);
    }

    public static string? GetRedisConnectionString(this IConfiguration configuration)
    {
        var vcapServices = configuration.GetVcapServices();

        if (vcapServices != null)
        {
            var redisCredentials = vcapServices?.Redis?.FirstOrDefault()?.Credentials;

            if (redisCredentials?.IsValid() == true)
            {
                return $"{redisCredentials.Host}:{redisCredentials.Port},password={redisCredentials.Password}";
            }
        }

        return configuration.GetConnectionString(EnvironmentVariables.FatpRedisConnectionString);
    }

    public static IServiceCollection AddLocationFilterService(this IServiceCollection services)
    {
        services.AddHttpClient<ILocationFilterService, PostcodesIoLocationFilterService>(client =>
        {
            client.BaseAddress = new Uri("https://api.postcodes.io");
        });

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITuitionPartnerService, TuitionPartnerService>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGeographyLookupRepository, GeographyLookupRepository>();
        services.AddScoped<ILookupDataRepository, LookupDataRepository>();
        services.AddScoped<ITuitionPartnerRepository, TuitionPartnerRepository>();
        return services;
    }

    public static IServiceCollection AddDataImporter(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GoogleDrive>(configuration.GetSection(nameof(GoogleDrive)));
        services.AddOptions();
        services.AddScoped<GoogleDriveServiceFactory>();
        services.AddScoped<IDataFileEnumerable, GoogleDriveDataFileEnumerable>();
        services.AddScoped<ILogoFileEnumerable, GoogleDriveLogoFileEnumerable>();
        services.AddScoped<ISpreadsheetExtractor, OpenXmlSpreadsheetExtractor>();
        services.AddScoped<ISpreadsheetTuitionPartnerFactory, SpreadsheetTuitionPartnerFactory>();
        services.AddScoped<IGeneralInformationAboutSchoolsRecords, GeneralInformatioAboutSchoolsRecords>();
        services.AddScoped<ISchoolsFactory, SchoolsFactory>();
        services.AddScoped<ITribalSpreadsheetTuitionPartnerFactory, TribalSpreadsheetTuitionPartnerFactory>();
        services.AddScoped<IQualityAssuredSpreadsheetTuitionPartnerFactory, QualityAssuredSpreadsheetTuitionPartnerFactory>();
        services.AddHostedService<DataImporterService>();
        return services;
    }
}