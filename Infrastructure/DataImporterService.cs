using System.Security.Cryptography;
using System.Text;
using Application.Extensions;
using Application.Factories;
using Domain;
using Domain.Validators;
using Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public class DataImporterService : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly DataEncryption _config;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public DataImporterService(IHostApplicationLifetime host, IOptions<DataEncryption> config, ILogger<DataImporterService> logger, IServiceScopeFactory scopeFactory)
    {
        _host = host;
        _config = config.Value;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_config.Key))
        {
            _logger.LogError("Importing encrypted files requires the --DataEncryption:Key argument e.g. --DataEncryption:Key \"I0YRt6YZrMvdTSN107O1R5b4lS16Gz7wBMMruEhqAJc=\". These can also be environment variables");

            _host.StopApplication();

            return;
        }

        var keyBytes = Convert.FromBase64String(_config.Key);

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NtpDbContext>();
        var factory = scope.ServiceProvider.GetRequiredService<ITuitionPartnerFactory>();

        _logger.LogWarning("Migrating database");
        await dbContext.Database.MigrateAsync(cancellationToken);

        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(
            async () =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                _logger.LogWarning("Deleting all existing Tuition Partner data");
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"LocalAuthorityDistrictCoverage\"", cancellationToken: cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"SubjectCoverage\"", cancellationToken: cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Prices\"", cancellationToken: cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"TuitionPartners\"", cancellationToken: cancellationToken);

                var assembly = typeof(AssemblyReference).Assembly;

                foreach (var resourceName in assembly.GetManifestResourceNames())
                {
                    if (resourceName.EndsWith("README.md")) break;

                    var base64Filename = resourceName.Substring(resourceName.LastIndexOf('.') + 1).FromBase64Filename();
                    var originalFilename = Encoding.UTF8.GetString(Convert.FromBase64String(base64Filename));
                    _logger.LogInformation($"Attempting to import original Tuition Partner spreadsheet {originalFilename}");

                    await using var resourceStream = assembly.GetManifestResourceStream(resourceName);
                    if (resourceStream == null)
                    {
                        _logger.LogError($"Tuition Partner resource name {resourceName} for file {originalFilename} not found");
                        continue;
                    }

                    _logger.LogInformation($"Attempting create Tuition Partner from file {originalFilename}");
                    TuitionPartner tuitionPartner;
                    try
                    {
                        using var stream = new MemoryStream();
                        using var crypto = Aes.Create();
                        var iv = new byte[crypto.IV.Length];
                        await resourceStream.ReadAsync(iv, 0, iv.Length, cancellationToken);
                        using var cryptoTransform = crypto.CreateDecryptor(keyBytes, iv);
                        await using var cryptoStream = new CryptoStream(resourceStream, cryptoTransform, CryptoStreamMode.Read);
                        await cryptoStream.CopyToAsync(stream, cancellationToken);
                        tuitionPartner = await factory.GetTuitionPartner(stream, cancellationToken);
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