using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using AppEnvironmentVariables = Infrastructure.Constants.EnvironmentVariables;

namespace Functions;

public class DataExtraction
{
    private readonly ILogger<DataExtraction> _logger;
    private readonly IConfiguration _configuration;

    public DataExtraction(ILogger<DataExtraction> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [Function("DataExtraction")]
    public async Task RunAsync([TimerTrigger("* * * * *")] MyInfo myTimer, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Find a Tuition Partner DataExtraction function executed at: {DateTime.UtcNow}");

        var dbConnectionString = _configuration.GetConnectionString(AppEnvironmentVariables.FatpDatabaseConnectionString);
        await using var connection = new NpgsqlConnection(dbConnectionString);
        try
        {
            await connection.OpenAsync(cancellationToken);

            // Here you can run some custom SQL query to check if database responds correctly
            await using var command = new NpgsqlCommand("SELECT 1;", connection);

            var result = await command.ExecuteScalarAsync(cancellationToken);

            var castResult = Convert.ToInt64(result);
            
            _logger.LogInformation("The query count is: {castResult}", castResult);
            
        }
        catch (NpgsqlException ex)
        {
            // In case of exception (database not available, wrong query, etc.) return Unhealthy
           _logger.LogError(ex.Message);
        }
    }
}