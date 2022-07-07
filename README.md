# Compare National Tutoring Options

## Introduction

The National Tutoring Programme's Compare National Tutoring Options service supports state-funded schools in their work to help young people who have had their education disrupted by coronavirus (COVID-19).

The service explains the three funded options for tutoring with two of those options, Academic Mentors and School-Led Tutoring being built by other suppliers. This service concentrates on the Find a Tuition Partner option providing the ability to search the full list of quality assured Tuition Partners using the school's location and tutoring needs.

## Further documentation

* [Architectural Decision Records (ADR)](docs/decisions)
* [Runbooks](docs/runbooks)

## Development environment setup

### Tooling requirements

* .NET 6
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* [NodeJS LTS](https://nodejs.org/en/download/)

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

You will also need to set the current data encryption key used to encrypt the data files in order to import them locally. Ask the other developers for the latest encryption key and add it as a .NET user secret with the following command.

```
dotnet user-secrets set "DataEncryption:Key" "<DATA_ENCRYPTION_KEY>" -p UI
```

The database migrations, seed data and Tuition Partner data is deployed by and running the data importer project. Either run the project via Visual Studio or with the following command.

```
dotnet run --project DataImporter import
```

#### Migrations

The service uses Entity Framework Code First migrations to create the database schema and seed the database. The following commands are for reference when managing migrations

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

### Running the application

The UI project should be the startup project. Either run the project via Visual Studio or with the following command.

```
dotnet run --project UI
```

You can then access the application on [https://localhost:7036/](https://localhost:7036/)

## Testing

### End To End Testing

From a command prompt, change to the `UI` directory and run `npm install` to install the dependencies. Then run one of the following commands:

* `npx cypress run` to run all Cypress end to end tests in a headless browser
* `npx cypress open` to open the Cypress test runner for fully manual configuration of the test runner
* `npx cypress open --config baseUrl=https://my-url/ --env username=<USERNAME>,password=<PASSWORD>` to open the Cypress test runner specifying a different base url and basic HTTP authentication credentials

### Accessibility Testing

The team currently use the following tools to aid manual accessibility testing

* [axe DevTools](https://www.deque.com/axe/devtools/)
* [WAVE](https://wave.webaim.org/)

### Security Testing

The team currently use the following tools to aid manual security testing

* [OWASP Zed Attack Proxy](https://www.zaproxy.org/)

## GOV.UK PaaS

Follow the [Cloud Foundry command line set up guide](https://docs.cloud.service.gov.uk/get_started.html#set-up-the-cloud-foundry-command-line) to support managing the deployments locally.

The [Manual Environment Setup](docs/runbooks/manual-environment-setup.md) runbook details the commands used.