using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Extensions;

public static class ServiceCollectionEnvironmentExtensions
{
    public static WebApplicationBuilder AddEnvironmentConfiguration(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Host.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"GoogleTagManager:ContainerId", "testing"},
                });
            });
        }

        return builder;
    }
}