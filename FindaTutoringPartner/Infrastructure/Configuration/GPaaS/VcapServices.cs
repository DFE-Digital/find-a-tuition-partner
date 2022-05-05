namespace Infrastructure.Configuration.GPaaS;

public class VcapServices
{
    public Postgres[]? Postgres { get; set; }
}

public class Postgres
{
    public Credentials? Credentials { get; set; }
}

public class Credentials
{
    public string? Host { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public int? Port { get; set; }
    public string? Username { get; set; }
}