---
# These are optional elements. Feel free to remove any of them.
status: accepted
date: 2023-04-18
deciders: NTP Dev Team, DfE Lead Developers
---
# Where to store and import TP spreadsheets from once Google Drive is no longer available

## Context and Problem Statement

DfE is migrating their 'Digital' accounts from Google Suite to Office 365. This means that the current method of 
importing the TP data from Google Drive will stop working. We therefore needed an alternative solution.

## Considered Options

* Switch to using OneDrive
* Store the TP data within the GitHub repository
* Use Azure BLOB Storage

## Decision Outcome

Chosen option: "Use Azure BLOB Storage", because it provides an isolated and dedicated place to store the TP data
which is simple to integrate with.

<!-- This is an optional element. Feel free to remove. -->
## Pros and Cons of the Options

### Switch to using OneDrive

This is making use of OneDrive in much the same way as Google Drive was used. Having the Tuition Partner
spreadsheets stored within a folder, and the importer accessing that folder via a service credential.

* Good, because it is easy for non-technical users to update the spreadsheets
* Goodm because OneDrive is already protected against malware
* Bad, because the service credential would need 'READ_ALL' permissions and could access all files in OneDrive

### Store the TP data within the GitHub repository

This is storing the files within the GitHub repository alongside the code. Updating the files would essentially
be a release and deployment.

* Good, because the respostory already exists
* Good, because files would be versioned and released as code
* Good, because the files being part of the deployment naturally allows QA of the files themselves through the lower envs.
* Bad, because the repository is public and there is a desire to keep the files themselves private

### Use Azure BLOB Storage

This is storing the files within a dedicated BLOB with Azure Blob Storage.

* Good, because the service credential can be locked down to the single BLOB store
* Good, because the data would be private
* Bad, because only users with access to Azure can update the files


<!-- markdownlint-disable-file MD013 -->
