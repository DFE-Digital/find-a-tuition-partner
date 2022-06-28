using Domain.Validators;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure;
using Infrastructure.Factories;
using Microsoft.EntityFrameworkCore;

namespace DataImporter;

public class DataImporterService : IHostedService
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public DataImporterService(ILogger<DataImporterService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NtpDbContext>();
        //dbContext.Database.d

        //Drop and recreate database each time this is run? No need for deltas? Yes - that seems simpler and more testable
        //await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.MigrateAsync(cancellationToken);

        //Replace with files in code
        //Files in code should be encrypted - use some kind of CLI syntax for this tool e.g. dataimporter prepare, dataimporter apply
        if (Directory.Exists(@"C:\Farsight"))
        {
            // Get only xlsx files from directory.
            var dirs = Directory.GetFiles(@"C:\Farsight", "*.xlsx");

            foreach (var fileName in dirs)
            {
                if (!File.Exists(fileName))
                {
                    _logger.LogError($"Tuition Partner file {fileName} not found");
                    continue;
                }

                await using var fileStream = File.OpenRead(fileName);
                var tuitionPartner = OpenXmlFactory.GetTuitionPartner(_logger, fileStream);
                if (tuitionPartner == null)
                {
                    _logger.LogError($"Could not create Tuition Partner from file {fileName}");
                    continue;
                }
                        
                var validator = new TuitionPartnerValidator();
                var results = await validator.ValidateAsync(tuitionPartner, cancellationToken);
                if (!results.IsValid)
                {
                    _logger.LogError($"Tuition Partner created from file {fileName} is not valid.{Environment.NewLine}{string.Join(Environment.NewLine, results.Errors)}");
                    continue;
                }

                dbContext.TuitionPartners.Add(tuitionPartner);
                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogWarning($"Added Tuition Partner {tuitionPartner.Name} with id of {tuitionPartner.Id} from file {fileName}");
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }
}