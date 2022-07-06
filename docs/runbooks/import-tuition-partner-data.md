---
Last verfied: 2022-07-04
---

# Importing Tuition Partner Data

## Description

The service relies on an import of Tuition Partner data from Excel spreadsheets they fill in and return to us. These are parsed by the code in the `DataImporter` project and added to the database.

The data importer also runs the migrations against the database prior to the import to update the schema.

## Data files

The Tuition Partner Excel spreadsheets returned by the Tuition Partners are checked for consistency and quality and placed in the `Quality Assured` folder in the ntp Google Drive. Is this these quality assured files that are imported.

## Steps

1. Navigate to the `Quality Assured` folder in the ntp Google Drive
2. Use the down arrow menu in the breadcrumb to download all the files
3. Remove any existing files in the `Infrastructure\Data` directory
4. Extract the contents of the downloaded `Quality Assured` folder into the `Infrastructure\Data` directory. The .xlsx files should not be in a subdirectory of that folder
5. Follow the steps below for a local or cloud deployment

### Local deployment

1. Either open up the solution and run the `DataImporter` project or run `dotnet run --project DataImporter` from the repo folder on the command line

### Cloud (GOV.UK PaaS) deployment

The following steps should be run from a command prompt or your terminal of choice.

1. Ensure that you have built the latest web assests by changing to the `UI` directory and running `npm install` followed by `npm run build`. Return to the root of the repo with `cd ..`
2. Log into GOV.UK PaaS `cf login -a api.london.cloud.service.gov.uk -u USERNAME` and select the destination space
3. Confirm the database backing service is present and available with by running `cf service national-tutoring-<ENVIRONMENT>-postgres-db`
4. If the database has not yet been created, provision a new instance with a command similar to `cf create-service postgres small-13 national-tutoring-<ENVIRONMENT>-postgres-db`
5. Run `cf push --strategy rolling --vars-file vars-<ENVIRONMENT>.yml` to deploy the app and the updated data files
6. Run `cf run-task national-tutoring-<ENVIRONMENT> --command "exec /home/vcap/deps/0/dotnet_publish/UI import --DataEncryption:Key <BASE64_ENCRYPTION_KEY>" --name national-tutoring-<ENVIRONMENT>-data-import` to start a task that will apply the migrations and update the Tuition Partner data

