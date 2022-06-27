using Infrastructure.Extensions;
using Infrastructure.Importers;
using Microsoft.Extensions.Hosting;
using Application;

namespace DataImporter
{
    internal class DataImporterService : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
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
