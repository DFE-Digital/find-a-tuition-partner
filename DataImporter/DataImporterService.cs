using Infrastructure.Importers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

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

                //Drop and recreate database each time this is run? No need for deltas? Yes - that seems simpler and more testable

                dbContext.Database.Migrate();

                //Replace with files in code
                //Files in code should be encrypted - use some kind of CLI syntax for this tool e.g. dataimporter prepare, dataimporter apply
                if (Directory.Exists(@"C:\Farsight"))
                {
                    // Get only xlsx files from directory.
                    string[] dirs = Directory.GetFiles(@"C:\Farsight", "*.xlsx");

                    foreach (string fileName in dirs)
                    {
                        //File exists check
                        //await using var fileStream = File.OpenRead(fileName);
                        //var tuitionPartner = OpenXmlFactory.GetTuitionPartner(fileStream);
                        //Null check
                        //var validator = new TuitionPartnerValidator();
                        //var result = validator.Validate(tuitionPartner);
                        //if(!results.IsValid) log out all validation errors
                        //else
                        //Find TP in database via name
                        //if existing
                        //Apply data updates - can be post private beta when we have an upload page
                        //If new
                        //Persist

                        NtpTutionPartnerExcelImporter.Import(fileName, dbContext, _logger);
                        await dbContext.SaveChangesAsync(cancellationToken);
                    }
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
