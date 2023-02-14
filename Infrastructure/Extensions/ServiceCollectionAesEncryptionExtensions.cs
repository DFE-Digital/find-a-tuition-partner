using Application.Common.Interfaces;
using Infrastructure.Configuration;
using Infrastructure.Encryption;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceCollectionAesEncryptionExtensions
{
    public static IServiceCollection AddAesEncryption(
        this IServiceCollection services, IConfiguration config)
    {
        services.Configure<EncryptionOptions>(
            config.GetSection(EncryptionOptions.Encryption));

        services.AddSingleton<IEncrypt, AesEncryption>();

        return services;
    }
}