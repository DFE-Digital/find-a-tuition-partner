using Application.DataImport;
using Application.Factories;
using Domain;
using Domain.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        var dataFileEnumerable = scope.ServiceProvider.GetRequiredService<IDataFileEnumerable>();
        var factory = scope.ServiceProvider.GetRequiredService<ITuitionPartnerFactory>();

        _logger.LogInformation("Migrating database");
        await dbContext.Database.MigrateAsync(cancellationToken);

        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(
            async () =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                _logger.LogInformation("Deleting all existing Tuition Partner data");
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"LocalAuthorityDistrictCoverage\"", cancellationToken: cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"SubjectCoverage\"", cancellationToken: cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Prices\"", cancellationToken: cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"TuitionPartners\"", cancellationToken: cancellationToken);

                foreach (var dataFile in dataFileEnumerable)
                {
                    var originalFilename = dataFile.Filename;

                    _logger.LogInformation($"Attempting to create Tuition Partner from file {originalFilename}");
                    TuitionPartner tuitionPartner;
                    try
                    {
                        tuitionPartner = await factory.GetTuitionPartner(dataFile.Stream, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Exception thrown when creating Tuition Partner from file {originalFilename}");
                        continue;
                    }

                    var validator = new TuitionPartnerValidator();
                    var results = await validator.ValidateAsync(tuitionPartner, cancellationToken);
                    if (!results.IsValid)
                    {
                        _logger.LogError($"Tuition Partner name {tuitionPartner.Name} created from file {originalFilename} is not valid.{Environment.NewLine}{string.Join(Environment.NewLine, results.Errors)}");
                        continue;
                    }

                    dbContext.TuitionPartners.Add(tuitionPartner);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation($"Added Tuition Partner {tuitionPartner.Name} with id of {tuitionPartner.Id} from file {originalFilename}");
                }

                await transaction.CommitAsync(cancellationToken);
            });

        _host.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}