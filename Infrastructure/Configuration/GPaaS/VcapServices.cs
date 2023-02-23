namespace Infrastructure.Configuration.GPaaS;

public class VcapServices
{
    public Postgres[]? Postgres { get; set; }
    public Redis[]? Redis { get; set; }
}

public class Postgres
{
    public PostgresCredentials? Credentials { get; set; }
}

public class Redis
{
    public RedisCredentials? Credentials { get; set; }
}

public class PostgresCredentials
{
    public string? Host { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public int? Port { get; set; }
    public string? Username { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Host) && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Password) && Port.HasValue && !string.IsNullOrEmpty(Username);
    }
}

public class RedisCredentials
{
    public string? Host { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public int? Port { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Host) && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Password) && Port.HasValue;
    }
}