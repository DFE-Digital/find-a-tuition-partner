﻿using Application;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.DataImport;
using Application.Extraction;
using Application.Factories;
using Infrastructure.Configuration;
using Infrastructure.Constants;
using Infrastructure.DataImport;
using Infrastructure.Extraction;
using Infrastructure.Factories;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

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
                options.ConfigurationOptions = ConfigurationOptions.Parse(connectionString);
                options.ConfigurationOptions.SyncTimeout = 15000;
                options.ConfigurationOptions.AsyncTimeout = 15000;
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        return services;
    }

    public static string GetNtpConnectionString(this IConfiguration configuration)
    {
        return configuration.GetConnectionString(EnvironmentVariables.FatpDatabaseConnectionString);
    }

    public static string? GetRedisConnectionString(this IConfiguration configuration)
    {
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
        services.AddScoped<IProcessEmailsService, ProcessEmailsService>();
        services.AddScoped<ILookupDataService, LookupDataService>();
        services.AddSingleton<IGenerateReferenceNumber, GenerateSupportReferenceNumber>();
        services.AddSingleton<IRandomTokenGenerator, RandomTokenGeneratorService>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ILocalAuthorityDistrictRepository, LocalAuthorityDistrictRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<ITuitionSettingRepository, TuitionSettingRepository>();
        services.AddScoped<ITuitionPartnerRepository, TuitionPartnerRepository>();
        services.AddScoped<IEnquiryRepository, EnquiryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }

    public static IServiceCollection AddDataImporter(this IServiceCollection services, IConfiguration configuration)
    {
        var azureBlobStorageSettings = new AzureBlobStorageSettings();
        configuration.GetSection(AzureBlobStorageSettings.BlobStorage).Bind(azureBlobStorageSettings);
        azureBlobStorageSettings.Validate();
        services.Configure<AzureBlobStorageSettings>(configuration.GetSection(AzureBlobStorageSettings.BlobStorage));
        services.AddOptions();
        services.AddScoped<IAzureBlobStorageService, AzureBlobStorageService>();
        services.AddScoped<IDataFileEnumerable, AzureBlobStorageDataFileEnumerable>();
        services.AddScoped<ILogoFileEnumerable, AzureBlobStorageLogoFileEnumerable>();
        services.AddScoped<ISpreadsheetExtractor, OpenXmlSpreadsheetExtractor>();
        services.AddScoped<IGeneralInformationAboutSchoolsRecords, GeneralInformatioAboutSchoolsRecords>();
        services.AddScoped<ISchoolsFactory, SchoolsFactory>();
        services.AddScoped<ITribalSpreadsheetTuitionPartnerFactory, TribalSpreadsheetTuitionPartnerFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddHttpClient<ILocationFilterService, PostcodesIoLocationFilterService>(client =>
        {
            client.BaseAddress = new Uri("https://api.postcodes.io");
        });
        services.AddHostedService<DataImporterService>();
        return services;
    }
}