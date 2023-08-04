using Dfe.Analytics.AspNetCore;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Infrastructure.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Analytics
{
    interface IAnalyticsConfigurerService
    {
        void ConfigureApp(IApplicationBuilder app);
    }

    public class AnalyticsConfigurerService : IAnalyticsConfigurerService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly bool _isConfigured;

        public AnalyticsConfigurerService(IConfiguration configuration, ILogger<AnalyticsConfigurerService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _isConfigured = IsAnalyticsConfigured();
        }

        public void ConfigureApp(IApplicationBuilder app)
        {
            if (_isConfigured)
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
            var dfeAnalyticsSettings = new DfeAnalyticsSettings();
            _configuration.GetSection(DfeAnalyticsSettings.DfeAnalyticsConfigName).Bind(dfeAnalyticsSettings);

            // Do we have the bare minimum of configuration?

            // `DatasetId` is always required
            if (string.IsNullOrEmpty(dfeAnalyticsSettings.DatasetId))
            {
                return false;
            }

            // `CredentialsJsonFile` is required
            if (string.IsNullOrEmpty(dfeAnalyticsSettings.CredentialsJsonFile))
            {
                return false;
            }

            // If using `CredentialsJsonFile` then `ProjectId` is required
            if (!string.IsNullOrEmpty(dfeAnalyticsSettings.CredentialsJsonFile) && string.IsNullOrEmpty(dfeAnalyticsSettings.ProjectId))
            {
                return false;
            }

            return true;
        }

        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            var dfeAnalyticsSettings = new DfeAnalyticsSettings();
            builder.Configuration.GetSection(DfeAnalyticsSettings.DfeAnalyticsConfigName).Bind(dfeAnalyticsSettings);

            var projectId = dfeAnalyticsSettings.ProjectId;
            var credentialsJsonFile = dfeAnalyticsSettings.CredentialsJsonFile;

            builder.Services.AddDfeAnalytics(options =>
            {
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Unknown";

                options.Environment = environmentName;

                if (!string.IsNullOrEmpty(credentialsJsonFile))
                {
                    var creds = GoogleCredential.FromFile(credentialsJsonFile);
                    options.BigQueryClient = BigQueryClient.Create(projectId, creds);
                }
            });
        }
    }
}

