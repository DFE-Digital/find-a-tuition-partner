# Release Process

## Introduction

This document covers the process from a developer having completed the work for a ticket though to that work being availiable to users on production.

## Approach

The team follows the concept of continuous deployment as closely as possible. This means that when a ticket is moved to the done column, the associated code should be deployed through to production shortly afterwards. There is no direct release approval process or bundling of completed tickets into a release only a reporting of what has been released via the done column.

This approach relies heavily on automation and end to end testing to support a manual per ticket sign off process as well as an alerting mechanism for early detection of errors and exceptions.

## Environments

There are four permenant environments, all of which are publicly available.

### QA

* [https://find-a-tuition-partner-qa.london.cloudapps.digital](https://find-a-tuition-partner-qa.london.cloudapps.digital)
* A commit to the main branch automatically triggers a deployment to the QA environment using the [Continuous Deployment to QA Environment](../.github/workflows/continuous-deployment-to-qa.yml) GitHub action
* Used for manual QA and regression tests

### User Research

* [https://find-a-tuition-partner-research.london.cloudapps.digital](https://find-a-tuition-partner-research.london.cloudapps.digital)
* Reserved exclusively for User Research
* Deployments to this environment should only be done with the approval of the User Research team

### Staging

* [https://find-a-tuition-partner-staging.london.cloudapps.digital](https://find-a-tuition-partner-staging.london.cloudapps.digital)
* Performance testing is run against this environment
* Used for external sign off e.g. UAT, ITHC, accessibility testing
* Environment used by Tuition Partners to sign off their data
* Final go no go for release to production should be performed on this environment

### Production

* [https://www.find-tuition-partner.service.gov.uk](https://www.find-tuition-partner.service.gov.uk)
* Also available on [https://find-a-tuition-partner-production.london.cloudapps.digital](https://find-a-tuition-partner-production.london.cloudapps.digital)

## Pull Request Deployments

After development work needed to complete a ticket is finished and a PR is raised, the [Pull Request Deployment and End to End Testing](../.github/workflows/pull-request.yml) action automatically creates an environment for this PR and deploys the PR code to this environment. This action takes around 20 minutes initially due to the time it takes to create a new database backing service. After deployment, the full end to end test suite is run against this new environment.

If the whole process is successful, the action will post details of the PR deployment to the #ntp-dev-team Slack channel. This includes the URL of the PR deployment which uses the format https://find-a-tuition-partner-pr-XXX.london.cloudapps.digital where XXX is the PR number.

This PR deployment URL is what will be shared with the wider team and used to review and sign off the work. Because PRs contain only the code for a single ticket, these PR deployments can be used to test and sign off tickets individually and in isolation. This removes the need to create release branches or revert changes from main.

The PR environment will be torn down when the PR is closed (merged) using the [Pull Request Teardown Deployment](../.github/workflows/pull-request-teardown.yml) action.

## Guide

1) Complete the development work required for the ticket
2) Raise a PR using the [pull request template](../.github/pull_request_template.md) (defines definition of done)
3) Move the ticket to the review column
4) Get your PR reviewed by a member of the dev team but **do not merge the PR until the functionality is reviewed and signed off by the wider team**
5) Share the link to the PR deployment created following step 2 with a member of the following disciplines where appropriate:
    1) Product Manager or Owner
    2) Business Analyst
    3) Service Design
    4) Content
6) Iterate until approval is given that the ticket has been implemented correctly. This can be expressed by the reviewers moving the ticket to done
7) Merge your PR into `main`
8) Wait for the triggered [Continuous Deployment to QA Environment](https://github.com/DFE-Digital/find-a-tuition-partner/actions/workflows/continuous-deployment-to-qa.yml) workflow run to complete and go green
9) Run the [Deploy to GOV.UK PaaS](https://github.com/DFE-Digital/find-a-tuition-partner/actions/workflows/deploy-to-gpaas.yml) workflow using the `main` branch, targetting `staging` and the optional `Run database migrations and import data` checkbox (if your PR includes database migrations or updated TP data) to deploy your code to the staging environment
10) Run through the search journey manually on the [staging environment](https://find-a-tuition-partner-staging.london.cloudapps.digital) to confirm to your satisfaction that the deployment has been successful
11) Run the [Deploy to GOV.UK PaaS](https://github.com/DFE-Digital/find-a-tuition-partner/actions/workflows/deploy-to-gpaas.yml) workflow using the `main` branch, targetting `production` and the optional `Run database migrations and import data` checkbox (if your PR includes database migrations or updated TP data) to deploy your code to the production environment
12) Run through the search journey manually on the [production environment](https://www.find-tuition-partner.service.gov.uk) to confirm to your satisfaction that the deployment has been successful
13) Add "Deployed to Production" as a comment in the ticket