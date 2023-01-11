using Dfe.Analytics.AspNetCore;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;

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

            return (!string.IsNullOrEmpty(section["CredentialsJsonFile"])
                && !string.IsNullOrEmpty(section["ProjectId"])
                && !string.IsNullOrEmpty(section["DatasetId"]));
        }

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("DfeAnalytics");

            var projectId = section["ProjectId"];
            var credentialsJsonFile = section["CredentialsJsonFile"];

            services.AddDfeAnalytics(options =>
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

