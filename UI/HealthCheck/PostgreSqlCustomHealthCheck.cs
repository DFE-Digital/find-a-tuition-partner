using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

public class PostgreSqlCustomHealthCheck : IHealthCheck
{
    private readonly string _connectionString;

    public PostgreSqlCustomHealthCheck(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        try
        {
            await connection.OpenAsync(cancellationToken);

            // Here you can run some custom SQL query to check if database responds correctly
            await using var command = new NpgsqlCommand("SELECT 1;", connection);

            var result = await command.ExecuteScalarAsync(cancellationToken);

            var castResult = Convert.ToInt64(result);

            return castResult == 1 ? HealthCheckResult.Healthy("Custom PostgreSQL check executed a query successfully.")
                : HealthCheckResult.Unhealthy("Custom PostgreSQL check failed to execute a query.");
        }
        catch (NpgsqlException ex)
        {
            // In case of exception (database not available, wrong query, etc.) return Unhealthy
            return HealthCheckResult.Unhealthy(
                "Failed to connect to PostgreSQL in the custom health check.",
                exception: ex);
        }
    }
}