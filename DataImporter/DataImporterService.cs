using Infrastructure.Extensions;
using Infrastructure.Importers;
using Microsoft.Extensions.Hosting;
using Application;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure;

namespace DataImporter
{
    public class DataImporterService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public DataImporterService(ILogger<DataImporterService> logger,
                                  IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NtpDbContext>();
            }

            if (Directory.Exists(@"C:\Farsight"))
            {
                // Get only xlsx files from directory.
                string[] dirs = Directory.GetFiles(@"C:\Farsight", "*.xlsx");

                foreach (string fileName in dirs)
                {
                    NtpTutionPartnerExcelImporter.Import(fileName);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
