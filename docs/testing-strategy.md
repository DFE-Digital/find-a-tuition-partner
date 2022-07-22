# Testing Strategy

## Introduction

This document covers the testing strategy and release gates required to promote a release between the five environments.

## Local Development

Developers are expected to run all tests, linting and code quality checks against their code prior to raising a PR.

- `dotnet test` (unit tests)
- Cypress end to end testing
    - Leverage containerised environment used by PR bulds (docker compose)
- WAVE for additional accessibility checks
- OWASP Zed Attack Proxy
- `dotnet format` (apply basic coding style rules)

## Pull Requests

The [pull request](../.github/workflows/pull-request.yml) workflow is run whenever a PR is created or updated. This will build the service, run `dotnet test`, use docker compose to spin up the service, create the database and populate it with seed data then run the end to end tests against this temporary stack. The end to end tests will also check for accessibiliy violations.

Pull requests cannot me merged into `main` without the pull request workflow having run successfully.

## QA

All commits to the `main` branch automatically trigger the [deploy to GPaaS](../.github/workflows/deploy-to-gpaas.yml) workflow.

- `Target environment for the deployment` = `qa`
- `Run database migrations and import data` = `true`
- Runs full suite of end to end tests
- Exception and performance anomaly alerts into Slack
- Manual QA and team sign off

## Staging

The [deploy to GPaaS](../.github/workflows/deploy-to-gpaas.yml) workflow is used to deploy to staging following manual QA and team sign off.

- Deployment from `main` branch
- Runs full suite of end to end tests
- Manual performance test run
- Exception and performance anomaly alerts into Slack
- Tuition Partners can review their latest data import on this environment
- Final sign off before release to production

## Production

The [deploy to GPaaS](../.github/workflows/deploy-to-gpaas.yml) workflow is used to deploy to staging following team and Tution Partner (if their data has changed) sign off.

- Deployment from `main` branch
- Runs full suite of end to end tests
- Exception and performance anomaly alerts into Slack

## Research

- Used in the user research process to trial new designs
- Deployed on an ad-hoc basis from the appropriate branch