﻿namespace UI.Analytics
{
    public static class AnalyticsExtensions
    {
        public static IServiceCollection AddAnalytics(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);

            AnalyticsConfigurerService.ConfigureServices(services, configuration);

            services.Add(new ServiceDescriptor(typeof(IAnalyticsConfigurerService), typeof(AnalyticsConfigurerService), ServiceLifetime.Singleton));

            return services;
        }

        public static IApplicationBuilder UseAnalytics(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.ApplicationServices.GetService<IAnalyticsConfigurerService>()?.ConfigureApp(app);

            return app;
        }
    }
}
