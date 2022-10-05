---
Last verfied: 2022-09-22
---

# Importing Tuition Partner Data

## Description

The service relies on an import of Tuition Partner data from Excel spreadsheets stored in Google Drive. These spreadsheets are maintained by the Business Analysts and Product Managers on the team. The [`DataImporterService`](/Infrastructure/DataImporterService.cs) class retrieves the spreadsheets directly from Google Drive, parses the data and creates a Tuition Partner from each one in the database.

The data importer also runs the migrations against the database prior to the import to update the schema.

## Data files and logos

The Tuition Partner Excel spreadsheets are managed in the 🗀 `NTP` ❱ `Tuition Partner Data & Information` ❱ `TP Data - Beta` ❱ `FINAL Data Responses` folder in Google Drive. The logos are stored in the 🗀 `NTP` > `Tuition Partner Data & Information` ❱ `TP Data - Beta` ❱ `Logos` folder using filenames that contain the kebab cased version of the Tuition Partner's name. The shared drive ID for the drive is stored in the following GitHub Actions secret, which is converted to environment variables when deployed:

* `GOOGLE_DRIVE_SHARED_DRIVE_ID` - "School Services" drive id

## Google Cloud project service account key

The service uses a Google Cloud service account to access the folders. This service account has been granted access to export and download the spreadsheets and logos in these folders. The service identifies as this service account using a key. This key is stored in the `GOOGLE_DRIVE_SERVICE_ACCOUNT_KEY` GitHub Actions secret and this secret is written out as a file during deployment.

### Rotating service account key

This is covered in the Secrets document in Google Drive.

## Runbook

1. Request to change Tuition Partner data is made and approved
2. BA or PO updates the relevant spreadsheet
3. Developer uses the [Run Database Migrations and Import Data](/.github/workflows/import-data.yml) action to import the data to the QA environment
4. BA or PO signs off the imported data
5. (Optional) the link to the Tuition Partner's details page is sent to the TP for them to sign off
6. Use the above action to import the data into the other environments culminating in production