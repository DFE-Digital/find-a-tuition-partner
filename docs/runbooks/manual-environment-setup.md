---
Last verfied: 2022-07-04
---

# Manual Environment Setup

## Description

Deploying to an environment to GOV.UK PaaS will eventually be done with terraform scripts. Currently it is a manual process to initially set up the environment followed by a somewhat automated release process.

## Prerequisits

The service is deployed to [GOV.UK PaaS](https://www.cloud.service.gov.uk/) and these deployments are managed using the Cloud Foundry command line. Ensure you have installed this by following [this guide](https://docs.cloud.service.gov.uk/get_started.html#set-up-the-cloud-foundry-command-line)

You will also need the current LTS version of [NodeJS](https://nodejs.org/en/download/) installed.

## Guide

### Naming conventions

The service's domain name is `find-a-tuition-partner` and that is used as the prefix for all deployed applications and backing services. This is followed by the environment abbreviation on all non production environments e.g. `find-a-tuition-partner-qa`, `find-a-tuition-partner-research` etc.

The guide uses the production names for all applications and backing services. Add the environment abbreviation as appropriate.

### Scaling

The production environment is scheduled to use a `medium-13` plan for the postgres backing service. Each environment will also have a minimum of three application instances. This guide uses the `small-13` plan to prevent accidentally creating multiple expensive backing services. Replace the plan with the one appropriate for the environment in question.

## Runbook

### Login

```
cf login -a api.london.cloud.service.gov.uk -u USERNAME
```

### Database

The service uses [PostgreSQL](https://docs.cloud.service.gov.uk/deploying_services/postgresql/) (postgres) as its backing database service.

Check if the service has already been created

```
cf service find-a-tuition-partner-<ENVIRONMENT>-postgres-db
```

If not, create the service. This will take around ten minutes

```
cf create-service postgres small-13 find-a-tuition-partner-<ENVIRONMENT>-postgres-db --wait
```

### Application

These commands must be run from the root of the repository.

Confirm that the latest web assets have been built and bundled

```
cd UI
npm install
npm run build
cd ..
```

Make sure you have followed steps 1-5 to copy the Tuition Partner data Excel spreadsheets from the [Importing Tuition Partner Data](import-tuition-partner-data.md) runbook. Do not run the task as that will be done later.

Deploy the application. This will create it if it's the first deployment. Use the correct vars-<ENVIRONMENT>.yml file for the target environment

```
cf push --strategy rolling --vars-file vars-<ENVIRONMENT>.yml
```

Run the data import task which will also run the migrations against the database

```
cf run-task find-a-tuition-partner-<ENVIRONMENT> --command "exec /home/vcap/deps/0/dotnet_publish/UI import --name find-a-tuition-partner-<ENVIRONMENT>-data-import
```

### Basic HTTP Authentication

The site is kept private during the private beta phase using basic HTTP authentication. This step will be removed when the service moves into the public beta phase.

Clone and deploy the basic HTTP authentication route application

```
git clone https://github.com/alphagov/paas-cf_basic_auth_route_service
cd paas-cf_basic_auth_route_service
cf push find-a-tuition-partner-<ENVIRONMENT>-auth-service-app --no-start
```

Configure chosen username and password for environment and start the service

```
cf set-env find-a-tuition-partner-<ENVIRONMENT>-auth-service-app AUTH_USERNAME <USERNAME>
cf set-env find-a-tuition-partner-<ENVIRONMENT>-auth-service-app AUTH_PASSWORD <PASSWORD>
cf start find-a-tuition-partner-<ENVIRONMENT>-auth-service-app
```

Create the basic HTTP authentication backing service

```
cf create-user-provided-service find-a-tuition-partner-<ENVIRONMENT>-auth-service -r https://find-a-tuition-partner-<ENVIRONMENT>-auth-service-app.london.cloudapps.digital
```

Bind the National Tutoring application's route to the basic HTTP authentication application

```
cf bind-route-service london.cloudapps.digital find-a-tuition-partner-<ENVIRONMENT>-auth-service --hostname find-a-tuition-partner-<ENVIRONMENT>
```
