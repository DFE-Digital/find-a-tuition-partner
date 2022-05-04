# National Tutoring Programme (NTP)

## Introduction

National Tutoring Programme's Find a Tutoring Partner service is currently in discovery. This respository will initially contain spikes and prototype work and will eventually contain the publically accessible code under an appropriate licence for the live service.

## Development environment setup

### Tooling requirements

You also require the [Entity Framework Core Tools for the .NET Command-Line Interface](https://www.nuget.org/packages/dotnet-ef/) to manage database migrations. Install or update these using the following commands:

```
dotnet tool install -g dotnet-ef
dotnet tool update -g dotnet-ef
```
docker run --name ntp -e POSTGRES_PASSWORD=Pa55word! -p 5432:5432 -d postgres:13
docker stop ntp

dotnet ef migrations add NameOfMigration

### GOV.UK PaaS

#### Commands

cf push
cf delete fatp-dev
cf create-service postgres tiny-unencrypted-13 fatp-dev-postgres-db
cf delete-service fatp-dev-postgres-db


https://github.com/DFE-Digital/national-tutoring-programme/settings/secrets/actions
