# Disaster Recovery

## Introduction

This runbook should be used in the event of total loss of the database backing service, the application instances or both. The commands used here target the production environment.

## Loss of the database backing service

The find a tuition partner service does not store any user data or provide an interface to update the data in the database.

The geographical data e.g. Region and Local Authority District lists are part of the database seed data applied when migrations are run. The tuition partner data comes from the encrypted data files commited to the repo. More information about the tuition parner data import process can be found in the [runbook](import-tuition-partner-data.md). This means that the database can be recreated on demand.

GOV.UK Platform as a Service provides automatic point in time backups for the prior seven days. The [PostgreSQL service backup](https://docs.cloud.service.gov.uk/deploying_services/postgresql/#postgresql-service-backup) documentation contains the guide for restoring these backups. It is however usually faster to recreate the database in the event that the database is corrupted or lost.

### Recreate database runbook

Login

```
cf login -a api.london.cloud.service.gov.uk -u USERNAME
```

Check if the service is still present

```
cf service find-a-tuition-partner-production-postgres-db
```

If so, consider deleting the service

```
cf delete-service find-a-tuition-partner-production-postgres-db
```

Create the service. This will take around ten minutes

```
cf create-service postgres small-13 find-a-tuition-partner-<ENVIRONMENT>-postgres-db --wait
```

Run the [Deploy to GOV.UK PaaS](https://github.com/DFE-Digital/find-a-tuition-partner/actions/workflows/deploy-to-gpaas.yml) workflow using the `main` branch, targetting `production` and check the optional `Run database migrations and import data` checkbox. This will redeploy the application instances, run the database migrations and import all the tuition partner data. After this the service should be back up and running.

## Loss of application instances

A loss of some application instances should be automatically resolved by GOV.UK Platform as a Service. If this does not happen, scaling the application down and up again can resolve many problems:

```
cf scale find-a-tuition-partner-production -i 1
cf scale find-a-tuition-partner-production -i 3
```

If this fails or all application instances have been lost, redeploying the application using the the [Deploy to GOV.UK PaaS](https://github.com/DFE-Digital/find-a-tuition-partner/actions/workflows/deploy-to-gpaas.yml) workflow will recreate the application instances.

If all else fails, consider removing the application and redeploying

```
cf delete find-a-tuition-partner-production
```
