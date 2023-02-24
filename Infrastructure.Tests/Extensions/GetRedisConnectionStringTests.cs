using System.Collections.Generic;
using FluentAssertions;
using Infrastructure.Constants;
using Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Infrastructure.Tests.Extensions;

public class GetRedisConnectionStringTests
{
    [Fact]
    public void UseCredentialsForRedisConnectionString_WhenVcapServicesConfigured()
    {
        var expected = $"{ScenarioConstants.RedisHost}:{ScenarioConstants.RedisPort},ssl=true,password={ScenarioConstants.RedisPassword}";

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {EnvironmentVariables.VcapServices, ScenarioConstants.VcapServicesJson}
            })
            .Build();

        var connectionString = configuration.GetRedisConnectionString();

        connectionString.Should().Be(expected);
    }

    [Fact]
    public void UseRedisConnectionString_WhenVcapServicesConfiguredButInvalid()
    {
        var expected = "expected-connection-string";

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {$"ConnectionStrings:{EnvironmentVariables.FatpRedisConnectionString}", expected},
                {EnvironmentVariables.VcapServices, ScenarioConstants.VcapServicesInvalidJson}
            })
            .Build();

        var connectionString = configuration.GetRedisConnectionString();

        connectionString.Should().Be(expected);
    }

    [Fact]
    public void UseRedisConnectionString_WhenVcapServicesNotConfigured()
    {
        var expected = "expected-connection-string";

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {$"ConnectionStrings:{EnvironmentVariables.FatpRedisConnectionString}", expected}
            })
            .Build();

        var connectionString = configuration.GetRedisConnectionString();

        connectionString.Should().Be(expected);
    }
}