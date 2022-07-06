using Domain.Validators;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Factories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class DataImporterService : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public DataImporterService(IHostApplicationLifetime host, ILogger<DataImporterService> logger, IServiceScopeFactory scopeFactory)
    {
        _host = host;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NtpDbContext>();

        _logger.LogWarning("Migrating database");
        await dbContext.Database.MigrateAsync(cancellationToken);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        _logger.LogWarning("Deleting all existing Tuition Partner data");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"TuitionPartnerCoverage\"", cancellationToken: cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"LocalAuthorityDistrictCoverage\"", cancellationToken: cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"SubjectCoverage\"", cancellationToken: cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Prices\"", cancellationToken: cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"TuitionPartners\"", cancellationToken: cancellationToken);

        //Files in code should be encrypted - use some kind of CLI syntax for this tool e.g. dataimporter prepare, dataimporter apply
        var assembly = typeof(AssemblyReference).Assembly;

        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            if (resourceName.EndsWith("README.md")) break;

            await using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                _logger.LogError($"Tuition Partner resource name {resourceName} not found");
                continue;
            }

            _logger.LogInformation($"Attempting create Tuition Partner from resource name {resourceName}");
            var tuitionPartner = OpenXmlFactory.GetTuitionPartner(_logger, stream, dbContext);
            if (tuitionPartner == null)
            {
                _logger.LogError($"Could not create Tuition Partner from resource name {resourceName}");
                continue;
            }

            var validator = new TuitionPartnerValidator();
            var results = await validator.ValidateAsync(tuitionPartner, cancellationToken);
            if (!results.IsValid)
            {
                _logger.LogError($"Tuition Partner created from resource name {resourceName} is not valid.{Environment.NewLine}{string.Join(Environment.NewLine, results.Errors)}");
                continue;
            }

            dbContext.TuitionPartners.Add(tuitionPartner);
            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogWarning($"Added Tuition Partner {tuitionPartner.Name} with id of {tuitionPartner.Id} from resource name {resourceName}");
        }

        await transaction.CommitAsync(cancellationToken);

        _host.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}