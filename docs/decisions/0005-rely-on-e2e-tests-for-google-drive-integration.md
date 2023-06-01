---
status: accepted
date: 2022-09-23
deciders: NTP developers
---
# Rely on End to End Testing for the Google Drive Integration Work

## Context and Problem Statement

Import of the Tuition Partner (TP) data is moving from encrypted spreadsheets stored in this repo to downloading/exporting them directly from the Google Drive folder(s). This has required a refactor of the data import code and a new implementation that uses the Google Drive API to authenticate and import the files. The Google Drive API code has challenges around stubbing or mocking and the value of unit testing the import code was questioned.

## Decision Outcome

Chosen option: "Do not unit test the Google Drive import code", because it is indirectly tested when running the end to end tests.

### Positive Consequences

* Reduced maintenance overhead in the event Google makes API changes
* Remove test update barriers to refactoring codebase
* Any move away from spreadsheet imports towards an admin portal would remove the need for this import code and related tests

### Negative Consequences

* Reduces code coverage
