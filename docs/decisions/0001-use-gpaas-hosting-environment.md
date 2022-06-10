---
status: proposed

date: 2022-05-27
---
# 1. Service Hosting Environment

## Context and Problem Statement

We need a publicly available cloud hosting environment to deploy our service to.

The [DfE Technical Guidance](https://technical-guidance.education.gov.uk/infrastructure/hosting/) suggests two options; Azure and GOV.UK Platform as a Service (GPaaS).

GPaaS appears to be a good fit for a smaller scale, microservices style service like Find a Tuition Partner.

## Decision Drivers

* Cost
* Team skills needed
* Onboarding speed
* Security accreditaion
* Flexibility to support future requirements

## Considered Options

* [Azure](https://azure.microsoft.com/en-gb/)
* [GOV.UK Platform as a Service](https://www.cloud.service.gov.uk/) (GPaaS)

## Decision Outcome

Chosen option: "GOV.UK Platform as a Service (GPaaS)", because

* Existing security accreditation
* Reduction in team DevOps needs
* Cost
* Our service will fit (See [Validation](#validation))

### Positive Consequences

* We have greater control over our hosting environment, choice and provision of backing services
* We are unlikely to need dedicated DevOps resource

<!-- This is an optional element. Feel free to remove. -->
### Negative Consequences

* We cannot integrate MI/BI solutions directly with the database

<!-- This is an optional element. Feel free to remove. -->
## Pros and Cons of the Options

### GOV.UK Platform as a Service (GPaaS)

* Good, because platform has already passed relevant accreditation and compliance certification processes related to GOV.UK services
* Good, because platform has proven and [publicly available reliability statistics](https://status.cloud.service.gov.uk/)
* Good, because GPaaS significantly reduces or eliminates the need for specialist infrastructure or DevOps skills
* Good, because Platform requirements encourage many architectural and coding best practices and patterns
* Good, because It is very cost effective when compared to other cloud providers
* Bad, because choice of [backing services](https://admin.london.cloud.service.gov.uk/marketplace) is limited although sufficient for our service's needs
* Bad, because there is no support for a functions as a service style serverless model
* Bad, because backing datastores are not available outside of a bound application. Any external data access such as for BI/MI must be done via an API or from a push based data replica

### Azure

* Good, because it supports a huge amount of features and services offering maximum flexibility
* Bad, because its complexity requires expertese and DevOps assistance not currently available in the team
* Bad, because users must have an education.gov.uk Microsoft account to authenticate with the service

## Validation

### Theoretical Evaluation

#### GOV.UK PaaS Terms of Use

The GPaaS Terms of Use specify only two requirements. Each will be addressed separately.

##### Application must follow the twelve-factor application principles

[GPaaS' interpretation](https://docs.cloud.service.gov.uk/architecture.html#12-factor-application-principles) of the standard [12factor.net](https://12factor.net/) principles provides good clarity on expectations for an app. Many of these are solid architectural principles in general and have the added benefit of ensuring that the application is cloud agnostic, scalable and easy to deploy. We have engineered the application with the following principles in mind:

* Stateless - The application stores no state in memory or in individual instances. All intermediate or persistent data is stored in a backing service, currently a postgres database. The data protection keys required for security measures such as anti request forgery token validation are stored in the backing service meaning there is no need for sticky sessions or similar
* Configured via environment variables - No configuration is stored as code in the repository. All configuration is applied either via secrets during deployment or extracted from the VCAP_SERVICES environment variable which contains backing services configuration. There is no difference between the code deployed to development environments and that deployed to production
* Concurrency - All application instances run separately and isolated from each other. There is no cross process communication allowing simple horizontal scaling
* Loose coupling - Application follows clean architecture principles using interfaces and abstraction frameworks such as Entity Framework Core. Dependency Injection configuration handles switching backing services

##### Store data classified as up to 'official'

The Tuition Partner data currently held in the service is in the public domain as is the possible Schools data the service may use. The latter would be an import of data made available publicly via the Get information about Schools service.

The reporting component for NTP which would cover the take up of tuition provision may well contain more sensitive pupil related data. This however is currently proposed as a wholly separate service which would consume TP data from the Find a Tuition Partner service and therefore isn't in consideration. **Providing the reporting service does not store pupil's names**, information that can be used to identify a pupil is classified as 'official' and therefore storing in GPaaS is theoretically acceptable. Children's names are within a special 'official-sensitive' category.

### Practical Evaluation

The team has successfully deployed a prototype version of the service to GPaaS via a GitHub Workflow. We therefore practically evaluated the following potential challenges:

* Gained access to GPaaS - We currently have access to the sandbox environment. The team needs to establish timelines for onboarding to the production environment
Tooling is cross platform - Verified that the Cloud Foundry command line interface can be installed on Windows and Linux. Other teams have confirmed MacOS compatibility
* .NET 6 support - Built a simple ASP.NET MVC application with hard coded data. Added manifest file, specified .NET 6 in the buildpack file and deployed using the cf push command
* Deployment pipeline support - Created a deploy-to-gpaas.yml GitHub Workflow which uses the `cf push --strategy rolling` command. This is triggered manually
* Backing service connectivity - Provisioned a postgres database backing service. Specified binding in the manifest file. Generated database migrations and seed data. Code to extract database connection details from the `VCAP_SERVICES` environment variable. Run migrations on startup (will be revised for production) and confirmed database read and write connectivity
* Postgres database functionality - Confirmed that all current and potential requirements for the backing datastore could be fulfilled by postgres. These include
    * Database migrations and seed data
    * The primary search implementation
    * Data protection services
    * Identity tables
* Application scaling - Scaled the application up to three instances and down through two to one instance with the `cf scale national-tutoring-dev -i 2` command

<!-- markdownlint-disable-file MD013 -->
