# Find a Tuition Partner

## Introduction

The National Tutoring Programme supports state-funded schools in their work to help young people who have had their education disrupted by coronavirus (COVID-19).

There are three funded options for tutoring; engaging Tuition Partners to supply professional tutors, using Academic Mentors and via School-Led Tutoring. Find a tuition partner is the service covering the first option and the remaining two options are being built by other suppliers. The three options will be explained and linked to from a set of GOV.UK pages connecting the programme together.

The code in this repository is for the find a tuition partner service. This service provides the ability to search the full list of quality assured Tuition Partners using the school's location and tutoring needs.

## Further documentation

* [Architectural Decision Records (ADR)](docs/decisions)
* [Runbooks](docs/runbooks)
* [C4 Diagrams](docs/uml)
* [Release Process](docs/release-process.md)
* [Disaster Recovery](docs/runbooks/disaster-recovery.md)

## Architecture

### C1 System Context Diagram

![C1 System Context Diagram](docs/uml/images/c1_system_context.png)

### C2 Container Level Diagram

![C2 Container Level Diagram](docs/uml/images/c2_container.png)

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
docker run --name find-a-tuition-partner-postgres-db -e POSTGRES_PASSWORD=<LOCAL_DEV_PASSWORD> -p 5432:5432 -d postgres:13
```

Please note that you will need to start the container from the Docker Desktop container tab every time you restart your machine.

You will need to register the database connection string for local development as a .NET user secret with the following command.

```
dotnet user-secrets set "ConnectionStrings:FatpDatabase" "Host=localhost;Username=postgres;Password=<LOCAL_DEV_PASSWORD>;Database=fatp" -p UI
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

## Logging and Metrics

The service uses [Serilog](https://github.com/serilog/serilog) to support logging structured event data. It is configured to write logs to the console as default and optionally write to a TCP sink for logit.io data source integration.

### logit.io

[logit.io](logit.io) provides the ELK stack (Elasticsearch, Logstash, and Kibana) and Grafana as a service. The service ships logs to the find a tuition partner stack in the NTP account which is what new developers and analysts should request access to.

Log interrogation is provided by OpenSearch and metrics dashboards are configured in Grafana. logit.io provides alerting within its service however Grafana can also be configured to send alerts if required.

If you need to test logit.io integration from your development environment, use the following command to add the neccessary user secret:

```
dotnet user-secrets set "AppLogging:TcpSinkUri" "<TLS_URL>" -p UI
```

### Google Analytics (GA4)

Google Analytics is used to track service traffic and usage. There is a separate property per environment with an associated data stream and therefore measurement id.

If you need to test Google Analytics integration from your development environment, use the following command to add the neccessary user secret:

```
dotnet user-secrets set "GoogleAnalytics:MeasurementId" "<MEASUREMENT_ID>" -p UI
```

## Testing

### End To End Testing

#### PR Workflow

The full suite of end to end tests is run automatically for every opened PR branch using [this workflow](/.github/workflows/pull-request.yml)

#### Running Locally

From a command prompt, change to the `UI` directory and run `npm install` to install the dependencies. Then run one of the following commands:

* `npx cypress run` to run all Cypress end to end tests in a headless browser
* `npx cypress open` to open the Cypress test runner for fully manual configuration of the test runner
* `npx cypress open --config baseUrl=https://my-url/ --env username=<USERNAME>,password=<PASSWORD>` to open the Cypress test runner specifying a different base url and basic HTTP authentication credentials

### Docker Compose

It is also possible to test the full stack from within docker using docker compose. This method supports easy setup and teardown of the database and can be a good way to test database migrations and updated data. This is also how the PR builds are tested for rapid feeback. The following commands will run all unit tests, start the stack, run the migrations, import the data and run the end to end tests

```
dotnet test
docker compose up --build -d
docker compose run -e DataEncryption:Key=<DATA_ENCRYPTION_KEY> web ./UI import
cd UI
npx cypress run --config baseUrl=http://localhost:8080/
cd ..
```

### Accessibility Testing

Axe has been integrated with the Cypress end to end tests using [cypress-axe](https://github.com/component-driven/cypress-axe). This provides a basic level of automated accessibility testing for every scenario in the suite. See [accessibility.js](/UI/cypress/support/step_definitions/accessibility.js) to understand how this is configured and run. Accessibility violations are logged to the browser's console meaning that diagnosing a violation requires running the tests locally and [viewing the error in the DevTools console](https://github.com/component-driven/cypress-axe#standard-output)

Please note: This automated accessibility testing is not sufficient to replace manual testing by the team and a full external accessibility audit. The dev team should also use the following tools locally to confirm there are no accessibility violations prior to QA

* [axe DevTools](https://www.deque.com/axe/devtools/)
* [WAVE](https://wave.webaim.org/)

### Security Testing

The team currently use the following tools to aid manual security testing

* [OWASP Zed Attack Proxy](https://www.zaproxy.org/)

## GOV.UK PaaS

Follow the [Cloud Foundry command line set up guide](https://docs.cloud.service.gov.uk/get_started.html#set-up-the-cloud-foundry-command-line) to support managing the deployments locally.

The [Manual Environment Setup](docs/runbooks/manual-environment-setup.md) runbook details the commands used.