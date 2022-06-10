namespace Infrastructure.Tests;

public class ScenarioConstants
{
    public const string PostgresHost = "localhost";
    public const int PostgresPort = 5432;
    public const string PostgresUsername = "postgres";
    public const string PostgresPassword = "secret";
    public const string PostgresDatabaseName = "ntp";

    public static readonly string VcapServicesJson = @"{
  ""postgres"": [
   {
    ""binding_name"": null,
    ""credentials"": {
     ""host"": """ + PostgresHost + @""",
     ""jdbcuri"": ""jdbc:postgresql://rdsbroker-66ecd739-2e98-401a-9e45-17938165be06.c7uewwm9qebj.eu-west-1.rds.amazonaws.com:5432/DATABASE_NAME?password=PASSWORD\u0026ssl=true\u0026user=USERNAME"",
     ""name"": """ + PostgresDatabaseName + @""",
     ""password"": """ + PostgresPassword + @""",
     ""port"": " + PostgresPort + @",
     ""uri"": ""postgres://USERNAME:PASSWORD@rdsbroker-66ecd739-2e98-401a-9e45-17938165be06.c7uewwm9qebj.eu-west-1.rds.amazonaws.com:5432/DATABASE_NAME"",
     ""username"": """ + PostgresUsername + @"""
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
 }";

    public static readonly string VcapServicesInvalidJson = @"{
  ""postgres"": [
   {
    ""binding_name"": null,
    ""credentials"": {
     ""jdbcuri"": ""jdbc:postgresql://rdsbroker-66ecd739-2e98-401a-9e45-17938165be06.c7uewwm9qebj.eu-west-1.rds.amazonaws.com:5432/DATABASE_NAME?password=PASSWORD\u0026ssl=true\u0026user=USERNAME"",
     ""name"": """ + PostgresDatabaseName + @""",
     ""password"": """ + PostgresPassword + @""",
     ""port"": " + PostgresPort + @",
     ""uri"": ""postgres://USERNAME:PASSWORD@rdsbroker-66ecd739-2e98-401a-9e45-17938165be06.c7uewwm9qebj.eu-west-1.rds.amazonaws.com:5432/DATABASE_NAME"",
     ""username"": """ + PostgresUsername + @"""
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
 }";
}