# National Tutoring Programme (NTP)

## Introduction

National Tutoring Programme's Find a Tutoring Partner service is currently in discovery. This respository will initially contain spikes and prototype work and will eventually contain the publically accessible code under an appropriate licence for the live service.

## Architectural Decision Records (ADR)

[adr.github.io](https://adr.github.io/)
[Markdown Any Decision Records (MADR)](https://adr.github.io/madr/)

[Our decisions](docs/decisions)

## Development environment setup

### Tooling requirements

You also require the [Entity Framework Core Tools for the .NET Command-Line Interface](https://www.nuget.org/packages/dotnet-ef/) to manage database migrations. Install or update these using the following commands:

```
dotnet tool install -g dotnet-ef
dotnet tool update -g dotnet-ef
```

```
docker run --name ntp -e POSTGRES_PASSWORD=Pa55word! -p 5432:5432 -d postgres:13
docker stop ntp
```

Database connection string for local development
```
dotnet user-secrets set "ConnectionStrings:NtpDatabase" "Host=localhost;Username=postgres;Password=Pa55word!;Database=ntp" -p UI
```

```
dotnet ef migrations add InitialCreate -p Infrastructure -s UI
dotnet ef migrations remove -p Infrastructure -s UI
dotnet ef database update -p Infrastructure -s UI
```

### GOV.UK PaaS

#### Commands

Login
```
cf login -a api.london.cloud.service.gov.uk -u nationaltutoring@digital.education.gov.uk
```

```
cf push --strategy rolling
cf tail -f national-tutoring-dev

cf delete national-tutoring-dev
cf create-service postgres tiny-unencrypted-13 national-tutoring-dev-postgres-db
cf service national-tutoring-dev-postgres-db
cf delete-service national-tutoring-dev-postgres-db
cf install-plugin conduit
cf conduit national-tutoring-dev-postgres-db -- psql

cf set-env national-tutoring-dev-auth-service AUTH_USERNAME private
cf set-env national-tutoring-dev-auth-service AUTH_PASSWORD beta
cf create-user-provided-service national-tutoring-dev-auth-app -r https://national-tutoring-dev-auth-service.london.cloudapps.digital
cf update-user-provided-service national-tutoring-dev-auth-app -r https://national-tutoring-dev-auth-service.london.cloudapps.digital
cf bind-route-service london.cloudapps.digital national-tutoring-dev-auth-app --hostname national-tutoring-dev

```


https://github.com/DFE-Digital/national-tutoring-programme/settings/secrets/actions

Accessibility Testing

Vulnerability and penetration testing

https://www.zaproxy.org/