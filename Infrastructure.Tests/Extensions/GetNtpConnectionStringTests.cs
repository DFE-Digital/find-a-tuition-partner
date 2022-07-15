using System.Collections.Generic;
using FluentAssertions;
using Infrastructure.Constants;
using Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Infrastructure.Tests.Extensions;

public class GetNtpConnectionStringTests
{
    [Fact]
    public void UseCredentialsForConnectionString_WhenVcapServicesConfigured()
    {
        var expected = $"Host={ScenarioConstants.PostgresHost};Port={ScenarioConstants.PostgresPort};Username={ScenarioConstants.PostgresUsername};Password={ScenarioConstants.PostgresPassword};Database={ScenarioConstants.PostgresDatabaseName}";

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {EnvironmentVariables.VcapServices, ScenarioConstants.VcapServicesJson}
            })
            .Build();

        var connectionString = configuration.GetNtpConnectionString();

        connectionString.Should().Be(expected);
    }

    [Fact]
    public void UseNtpConnectionString_WhenVcapServicesConfiguredButInvalid()
    {
        var expected = "expected-connection-string";

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {$"ConnectionStrings:{EnvironmentVariables.FatpDatabaseConnectionString}", expected},
                {EnvironmentVariables.VcapServices, ScenarioConstants.VcapServicesInvalidJson}
            })
            .Build();

        var connectionString = configuration.GetNtpConnectionString();

        connectionString.Should().Be(expected);
    }

    [Fact]
    public void UseNtpConnectionString_WhenVcapServicesNotConfigured()
    {
        var expected = "expected-connection-string";

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {$"ConnectionStrings:{EnvironmentVariables.FatpDatabaseConnectionString}", expected}
            })
            .Build();

        var connectionString = configuration.GetNtpConnectionString();

        connectionString.Should().Be(expected);
    }
}