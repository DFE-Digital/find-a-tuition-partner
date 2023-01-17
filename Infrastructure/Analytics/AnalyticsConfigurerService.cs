using Dfe.Analytics.AspNetCore;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace UI.Analytics
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
            this._configuration = configuration;
            this._logger = logger;
            this._isConfigured = IsAnalyticsConfigured();
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
            var section = _configuration.GetSection("DfeAnalytics");

            // Do we have the bare minimum of configuration?

            // `DatasetId` is always required
            if (string.IsNullOrEmpty(section["DatasetId"]))
            {
                return false;
            }

            // One of `CredentialsJson` or `CredentialsJsonFile` is required
            if (string.IsNullOrEmpty(section["CredentialsJson"]) && string.IsNullOrEmpty(section["CredentialsJsonFile"]))
            {
                return false;
            }

            // If using `CredentialsJsonFile` then `ProjectId` is required
            if (!string.IsNullOrEmpty(section["CredentialsJsonFile"]) && string.IsNullOrEmpty(section["ProjectId"]))
            {
                return false;
            }

            return true;
        }

        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            var section = builder.Configuration.GetSection("DfeAnalytics");

            var projectId = section["ProjectId"];
            var credentialsJsonFile = section["CredentialsJsonFile"];

            builder.Services.AddDfeAnalytics(options =>
            {
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Unknown";

                options.Environment = environmentName;

                if (!string.IsNullOrEmpty(credentialsJsonFile))
                {
                    var creds = GoogleCredential.FromFile(section["CredentialsJsonFile"]);
                    options.BigQueryClient = BigQueryClient.Create(projectId, creds);
                }
            });
        }
    }
}

