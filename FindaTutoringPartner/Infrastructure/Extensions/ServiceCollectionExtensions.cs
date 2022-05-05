using System.Text.Json;
using Application;
using Infrastructure.Configuration.GPaaS;
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
            options.UseNpgsql(configuration.GetConnectionString("NtpDatabase"), action => action.MigrationsAssembly(typeof(AssemblyReference).Assembly.FullName));
        });

        services.AddScoped<INtpDbContext>(provider => provider.GetService<NtpDbContext>()!);

        return services;
    }

    public static IServiceCollection AddAddressLookup(this IServiceCollection services)
    {
        services.AddScoped<IAddressLookup, HardCodedAddressLookup>();

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
        }



        /*var databaseUrl = configuration["REDIS_URL"];
        //var connectionString = configuration.GetConnectionString("NtpDatabase");

        var vcapServicesDefined = !string.IsNullOrEmpty(Configuration["VCAP_SERVICES"]);
        var redisUrlDefined = !string.IsNullOrEmpty(Configuration["REDIS_URL"]);

        if (!vcapServicesDefined && !redisUrlDefined)
        {
            return;
        }

        var redisPass = "";
        var redisHost = "";
        var redisPort = "";
        var redisTls = false;

        if (!string.IsNullOrEmpty(Configuration["VCAP_SERVICES"]))
        {
            var vcapConfiguration = JObject.Parse(Configuration["VCAP_SERVICES"]);
            var redisCredentials = vcapConfiguration["redis"]?[0]?["credentials"];
            redisPass = (string)redisCredentials?["password"];
            redisHost = (string)redisCredentials?["host"];
            redisPort = (string)redisCredentials?["port"];
            redisTls = (bool)redisCredentials?["tls_enabled"];
        }
        else if (!string.IsNullOrEmpty(Configuration["REDIS_URL"]))
        {
            var redisUri = new Uri(Configuration["REDIS_URL"]);
            redisPass = redisUri.UserInfo.Split(":")[1];
            redisHost = redisUri.Host;
            redisPort = redisUri.Port.ToString();
        }*/

        return configuration.GetConnectionString("NtpDatabase");
    }
}