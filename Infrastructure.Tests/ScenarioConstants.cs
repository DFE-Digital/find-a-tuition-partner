namespace Infrastructure.Tests;

public class ScenarioConstants
{
    public const string PostgresHost = "localhost";
    public const int PostgresPort = 5432;
    public const string PostgresUsername = "postgres";
    public const string PostgresPassword = "secret";
    public const string PostgresDatabaseName = "ntp";

    public const string RedisHost = "localhost";
    public const int RedisPort = 6379;
    public const string RedisUsername = "Redisusername";
    public const string RedisPassword = "Redissecret";
    public const string RedisDatabaseName = "RedisDb";

    public static readonly string VcapServicesJson = @"{
  ""postgres"": [
    {
      ""binding_guid"": ""b56991ec-bad0-40f4-b48c-8c4e5acf6ad2"",
      ""binding_name"": null,
      ""credentials"": {
        ""host"": """ + PostgresHost + @""",
        ""jdbcuri"": ""jdbc:postgresql://rdsbroker-e3dc0dca-3452-4eef-993a-8096fcdde97b.coowcrpgh5fz.eu-west-2.rds.amazonaws.com:5432/DATABASE_NAME?password=PASSWORD\u0026ssl=true\u0026user=USERNAME"",
        ""name"": """ + PostgresDatabaseName + @""",
        ""password"": """ + PostgresPassword + @""",
        ""port"": " + PostgresPort + @",
        ""uri"": ""postgres://USERNAME:PASSWORD@rdsbroker-e3dc0dca-3452-4eef-993a-8096fcdde97b.coowcrpgh5fz.eu-west-2.rds.amazonaws.com:5432/DATABASE_NAME"",
        ""username"": """ + PostgresUsername + @"""
      },
      ""instance_guid"": ""e3dc0dca-3452-4eef-993a-8096fcdde97b"",
      ""instance_name"": """ + PostgresDatabaseName + @""",
      ""label"": ""postgres"",
      ""name"": """ + PostgresDatabaseName + @""",
      ""plan"": ""small-13"",
      ""provider"": null,
      ""syslog_drain_url"": null,
      ""tags"": [
        ""postgres"",
        ""relational""
      ],
      ""volume_mounts"": []
    }
  ],
  ""redis"": [
    {
      ""binding_guid"": ""55ec27ad-4eb9-49e2-a9b9-70b812648141"",
      ""binding_name"": null,
      ""credentials"": {
        ""host"": """ + RedisHost + @""",
        ""name"": """ + RedisDatabaseName + @""",
        ""password"": """ + RedisPassword + @""",
        ""port"": " + RedisPort + @",
        ""tls_enabled"": true,
        ""uri"": ""rediss://:USERNAME:PASSWORD@master.cf-hbwagvytebt5g.b5xrdh.euw2.cache.amazonaws.com:6379""
      },
      ""instance_guid"": ""e6d8f17a-8664-4b54-942b-e626712f406c"",
      ""instance_name"": """ + RedisDatabaseName + @""",
      ""label"": ""redis"",
      ""name"": """ + RedisDatabaseName + @""",
      ""plan"": ""tiny-6.x"",
      ""provider"": null,
      ""syslog_drain_url"": null,
      ""tags"": [
        ""elasticache"",
        ""redis""
      ],
      ""volume_mounts"": []
    }
  ]
}
";

    public static readonly string VcapServicesInvalidJson = @"{
  ""postgres"": [
    {
      ""binding_guid"": ""b56991ec-bad0-40f4-b48c-8c4e5acf6ad2"",
      ""binding_name"": null,
      ""credentials"": {
        ""jdbcuri"": ""jdbc:postgresql://rdsbroker-e3dc0dca-3452-4eef-993a-8096fcdde97b.coowcrpgh5fz.eu-west-2.rds.amazonaws.com:5432/DATABASE_NAME?password=PASSWORD\u0026ssl=true\u0026user=USERNAME"",
        ""name"": """ + PostgresDatabaseName + @""",
        ""password"": """ + PostgresPassword + @""",
        ""port"": " + PostgresPort + @",
        ""uri"": ""postgres://USERNAME:PASSWORD@rdsbroker-e3dc0dca-3452-4eef-993a-8096fcdde97b.coowcrpgh5fz.eu-west-2.rds.amazonaws.com:5432/DATABASE_NAME"",
        ""username"": """ + PostgresUsername + @"""
      },
      ""instance_guid"": ""e3dc0dca-3452-4eef-993a-8096fcdde97b"",
      ""instance_name"": """ + PostgresDatabaseName + @""",
      ""label"": ""postgres"",
      ""name"": """ + PostgresDatabaseName + @""",
      ""plan"": ""small-13"",
      ""provider"": null,
      ""syslog_drain_url"": null,
      ""tags"": [
        ""postgres"",
        ""relational""
      ],
      ""volume_mounts"": []
    }
  ],
  ""redis"": [
    {
      ""binding_guid"": ""55ec27ad-4eb9-49e2-a9b9-70b812648141"",
      ""binding_name"": null,
      ""credentials"": {
        ""name"": """ + RedisDatabaseName + @""",
        ""password"": """ + RedisPassword + @""",
        ""port"": " + RedisPort + @",
        ""tls_enabled"": true,
        ""uri"": ""rediss://:USERNAME:PASSWORD@master.cf-hbwagvytebt5g.b5xrdh.euw2.cache.amazonaws.com:6379""
      },
      ""instance_guid"": ""e6d8f17a-8664-4b54-942b-e626712f406c"",
      ""instance_name"": """ + RedisDatabaseName + @""",
      ""label"": ""redis"",
      ""name"": """ + RedisDatabaseName + @""",
      ""plan"": ""tiny-6.x"",
      ""provider"": null,
      ""syslog_drain_url"": null,
      ""tags"": [
        ""elasticache"",
        ""redis""
      ],
      ""volume_mounts"": []
    }
  ]
}";
}