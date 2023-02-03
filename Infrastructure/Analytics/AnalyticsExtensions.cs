using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Analytics
{
    public static class AnalyticsExtensions
    {
        public static WebApplicationBuilder AddAnalytics(this WebApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            AnalyticsConfigurerService.ConfigureServices(builder);

            builder.Services.Add(new ServiceDescriptor(typeof(IAnalyticsConfigurerService), typeof(AnalyticsConfigurerService), ServiceLifetime.Singleton));

            return builder;
        }

        public static IApplicationBuilder UseAnalytics(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.ApplicationServices.GetService<IAnalyticsConfigurerService>()?.ConfigureApp(app);

            return app;
        }
    }
}

