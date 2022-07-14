---
Last verfied: 2022-07-06
---

# Importing Tuition Partner Data

## Description

The service relies on an import of Tuition Partner data from Excel spreadsheets they fill in and return to us. These are parsed by the code in the `DataImporter` project and added to the database.

The data importer also runs the migrations against the database prior to the import to update the schema. This runbook explains how to encrypt and update the data files in source and deploy them to an environment.

Updating the data files and importing them again is the current method of updating the Tuition Partner data. The plan for the future is to replace this with a file upload function protected by a login.

## Data files

The Tuition Partner Excel spreadsheets returned by the Tuition Partners are checked for consistency and quality and placed in the `QA Responses` folder in the ntp Google Drive. It is this these quality assured files that are imported.

## Encryption

The data files are encrypted using the Advanced Encryption Standard (AES) symmetric block cipher. Once encrypted, these files are safe to commit to a public repository. It is important to note that there is no sensitive data in these files. They only contain data that will be publicly accessible via the service. It is however best practice to encrypt data at rest which is why we have chosen to do so.

## Runbook

### Generating a new encryption key

The encryption key should be rotated regularly and when development team members leave. The data importer application is used to generate new keys which then need to be used and stored. Please note, you do not have to rotate the key on every import.

Generate a new key

```
dotnet run --project DataImporter generate-key
```

You will see messages showing the key and the parameters you will need to use that key when encrypting the files. Once you have encrypted the data files in source using this key you will need to perform some further actions:

1. Update your local key by running `dotnet user-secrets set "DataEncryption:Key" "<DATA_ENCRYPTION_KEY>" -p UI`
2. Use [https://onetimesecret.com/](https://onetimesecret.com/) to share the new key with the developers
3. Update the `DATA_ENCRYPTION_KEY` [GitHub secret](settings/secrets/actions)

### Update the data files

The data files commited to the repo need updating when the Tuition Partner data changes. This is the first step in updating the Tuition Partner data on the live service.

1. Navigate to the `QA Responses` folder in the ntp Google Drive
2. Use the down arrow menu in the breadcrumb to download all the files
3. Extract the contents of the downloaded zip file to a directory
4. Copy the full path of the directory containing the .xlsx files

You can now encrypt the data files and copy them into the repo by running the following command

```
dotnet run --project DataImporter encrypt --DataEncryption:SourceDirectory "<QA_RESPONSES_DIRECTORY>"
```

This will pick up your `DataEncryption:Key` user secret. If you want to override this, add `--DataEncryption:Key "<DATA_ENCRYPTION_KEY>"` to the previous command.

Finally ensure you commit the new files and push the changes.

### Local deployment

Ensure you test the data files locally by running the following command to import the new data

```
dotnet run --project DataImporter import
```

### Cloud (GOV.UK PaaS) deployment

Use the Deploy to GOV.UK PaaS and select the `Run database migrations and import data` option to import the data into the target environment.