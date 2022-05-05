using System.Collections.Generic;
using Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Infrastructure.Tests.Extensions
{
    public class GetNtpConnectionStringTests
    {
        [Fact]
        public void WhenVcapServicesConfigured()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"VCAP_SERVICES", @"{
  ""postgres"": [
   {
    ""binding_name"": null,
    ""credentials"": {
     ""host"": ""rdsbroker-66ecd739-2e98-401a-9e45-15436165be06.c7uewwm9qebj.eu-west-1.rds.amazonaws.com"",
     ""jdbcuri"": ""jdbc:postgresql://rdsbroker-66ecd739-2e98-401a-9e45-17938165be06.c7uewwm9qebj.eu-west-1.rds.amazonaws.com:5432/DATABASE_NAME?password=PASSWORD\u0026ssl=true\u0026user=USERNAME"",
     ""name"": ""DATABASE_NAME"",
     ""password"": ""PASSWORD"",
     ""port"": 5432,
     ""uri"": ""postgres://USERNAME:PASSWORD@rdsbroker-66ecd739-2e98-401a-9e45-17938165be06.c7uewwm9qebj.eu-west-1.rds.amazonaws.com:5432/DATABASE_NAME"",
     ""username"": ""USERNAME""
    },
    ""instance_name"": ""SERVICE_NAME"",
    ""label"": ""postgres"",
    ""name"": ""SERVICE_NAME"",
    ""plan"": ""PLAN"",
    ""provider"": null,
    ""syslog_drain_url"": null,
    ""tags"": [
     ""postgres"",
     ""relational""
    ],
    ""volume_mounts"": []
   }
  ]
 }"}
                })
                .Build();

            var connectionString = configuration.GetNtpConnectionString();

        }
    }
}