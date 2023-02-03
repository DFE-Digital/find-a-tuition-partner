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

        return configuration.GetConnectionString(EnvironmentVariables.FatpDatabaseConnectionString);
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
        services.AddScoped<ILookupDataService, LookupDataService>();

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
        services.AddScoped<ITuitionPartnerFactory, QualityAssuredSpreadsheetTuitionPartnerFactory>();
        services.AddScoped<IGeneralInformationAboutSchoolsRecords, GeneralInformatioAboutSchoolsRecords>();
        services.AddScoped<ISchoolsFactory, SchoolsFactory>();
        services.AddHostedService<DataImporterService>();
        return services;
    }
}