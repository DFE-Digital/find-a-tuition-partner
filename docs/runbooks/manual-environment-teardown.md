---
Last verfied: 2022-07-04
---

# Manual Environment Teardown

## Description

This runbook covers the manual teardown of a complete environment in the event of a failure to remove a PR build or the decommission of an environment.

## Prerequisits

The service is deployed to [GOV.UK PaaS](https://www.cloud.service.gov.uk/) and these deployments are managed using the Cloud Foundry command line. Ensure you have installed this by following [this guide](https://docs.cloud.service.gov.uk/get_started.html#set-up-the-cloud-foundry-command-line)

## Guide

### Naming conventions

The service's domain name is `find-a-tuition-partner` and that is used as the prefix for all deployed applications and backing services. This is followed by the environment abbreviation on all non production environments e.g. `find-a-tuition-partner-qa`, `find-a-tuition-partner-research` etc.

The guide uses the production names for all applications and backing services. Add the environment abbreviation as appropriate.

## Runbook

### Login

```
cf login -a api.london.cloud.service.gov.uk -u USERNAME
```

### Application

Remove the application first to prevent any access to it.

```
cf delete find-a-tuition-partner-<ENVIRONMENT> -r
```

### Database

Remove the backing database service.

```
cf delete-service find-a-tuition-partner-<ENVIRONMENT>-postgres-db
```

### Basic HTTP Authentication

Remove the basic authentication application and backing service if the environment was protected via basic HTTP authentication.

Remove the basic HTTP authentication route app

```
cf delete find-a-tuition-partner-<ENVIRONMENT>-auth-service-app -r
```

Remove the basic HTTP authentication backing service

```
cf delete-service find-a-tuition-partner-<ENVIRONMENT>-auth-service
```
