using System;
using Dfe.Analytics.AspNetCore;

namespace UI.Analytics
{
    internal interface IAnalyticsConfigurerService
    {
        void ConfigureApp(IApplicationBuilder app);
    }

    internal class AnalyticsConfigurerService : IAnalyticsConfigurerService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly bool _isConfigured;

        public AnalyticsConfigurerService(IConfiguration configuration, ILogger<AnalyticsConfigurerService> logger)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._isConfigured = IsAnalyticsConfigured();
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddDfeAnalytics(ConfigureDfeAnalyticsOptions);
        }

        public void ConfigureApp(IApplicationBuilder app)
        {
            if (this.IsAnalyticsConfigured())
            {
                app.UseDfeAnalytics();
            }
            else
            {
                _logger.LogWarning("DfeAnalytics is NOT configured and will be disabled");
            }
        }

        private bool IsAnalyticsConfigured()
        {
            var section = _configuration.GetSection("DfeAnalytics");

            return (!string.IsNullOrEmpty(section["CredentialsJson"]));
        }

        private static void ConfigureDfeAnalyticsOptions(DfeAnalyticsOptions options)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Unknown";

            options.Environment = environmentName;
            options.DatasetId = $"fatp_events_{environmentName.ToLower()}";
        }
    }
}

