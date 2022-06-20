# National Tutoring Programme (NTP)

## Introduction

National Tutoring Programme's Find a Tutoring Partner service is currently in discovery. This respository will initially contain spikes and prototype work and will eventually contain the publically accessible code under an appropriate licence for the live service.

## Architectural Decision Records (ADR)

[adr.github.io](https://adr.github.io/)
[Markdown Any Decision Records (MADR)](https://adr.github.io/madr/)

[Our decisions](docs/decisions)

## Development environment setup

### Tooling requirements

* .NET 6
* Docker Desktop
* NodeJS 16

You also require the [Entity Framework Core Tools for the .NET Command-Line Interface](https://www.nuget.org/packages/dotnet-ef/) to manage database migrations. Install or update these using the following commands:

```
dotnet tool install -g dotnet-ef
dotnet tool update -g dotnet-ef
```

### Database

The service uses Postgres 13 for the database backing service. It is recommended that you use a pre built docker image for local development. Run the following command to start the container.

```
docker run --name ntp -e POSTGRES_PASSWORD=<LOCAL_DEV_PASSWORD> -p 5432:5432 -d postgres:13
```

Please note that you will need to start the container from the Docker Desktop container tab every time you restart your machine.

You will need to register the database connection string for local development as a .NET user secret with the following command.
```
dotnet user-secrets set "ConnectionStrings:NtpDatabase" "Host=localhost;Username=postgres;Password=<LOCAL_DEV_PASSWORD>;Database=ntp" -p UI
```

#### Migrations

The service uses Entity Framework Code First migrations to create the database schema and seed the database. These are currently run when the UI first starts (this will change in future) so the following commands are for reference when managing migrations

```
dotnet ef migrations add InitialCreate -p Infrastructure -s UI
dotnet ef migrations remove -p Infrastructure -s UI
dotnet ef database update -p Infrastructure -s UI
```

### Web assets

From a command prompt, change to the `UI` directory and run `npm install` to install the dependencies. Then run one of the following commands:

* `npm run build` to bundle the assets using webpack 5 in production mode
* `npm run build:dev` to bundle the assets using webpack 5 in development mode
* `npm run watch` to bundle the assets using webpack 5 in development mode and apply changes immediately when developing

## Testing

### End To End Testing

From a command prompt, change to the `End-2-End-Testing` directory and run `npm install` to install the dependencies. Then run one of the following commands:

* `npx cypress run` to run all Cypress end to end tests in a headless browser
* `npx cypress open` to open the Cypress test runner for fully manual configuration of the test runner
* `npx cypress open --config baseUrl=https://my-url/ --env username=private,password=beta` to open the Cypress test runner specifying a different base url and basic HTTP authentication credentials

## GOV.UK PaaS

Follow the [Cloud Foundry command line set up guide](https://docs.cloud.service.gov.uk/get_started.html#set-up-the-cloud-foundry-command-line) to support managing the deployments locally.

### Commands

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

cf set-env national-tutoring-dev-auth-service AUTH_USERNAME <HTTP_AUTH_USERNAME>
cf set-env national-tutoring-dev-auth-service AUTH_PASSWORD <HTTP_AUTH_PASSWORD>
cf create-user-provided-service national-tutoring-dev-auth-app -r https://national-tutoring-dev-auth-service.london.cloudapps.digital
cf update-user-provided-service national-tutoring-dev-auth-app -r https://national-tutoring-dev-auth-service.london.cloudapps.digital
cf bind-route-service london.cloudapps.digital national-tutoring-dev-auth-app --hostname national-tutoring-dev

```


https://github.com/DFE-Digital/national-tutoring-programme/settings/secrets/actions

Accessibility Testing

Vulnerability and penetration testing

https://www.zaproxy.org/
