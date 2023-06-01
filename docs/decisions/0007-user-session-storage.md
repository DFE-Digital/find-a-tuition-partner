---
status: accepted
date: 2023-02-17
deciders: NTP Dev Team
---
# How to store temporary user session data

## Context and Problem Statement

Currently the service builds the user's input (search criteria) in the query string. This works well for the current search use-case because the data is small and not sensitive.

However, for the Build an Enquiry feature we will need to store larger amounts of data, and it will contain some personal data such as work email address. We therefore require a different solution to store temporary user-entered data while they are going through the journey, but before it is finally submitted into the PostgreSQL DB.

## Decision Drivers

* Some data will be personal data
* We have a short implementation deadline so introducing new infrastructure is problematic
* Data is text data, and will potentially be several kB in size
* We need to maintain horizontal scalability of the application

## Considered Options

* Use Redis as a dedicated shared cache
* Use the existing PostgreSQL database
* Use an in-process in-memory store, with session affinity 

## Decision Outcome

Chosen option: "Use Redis as a dedicated shared cache", because
it is a standard pattern, supported ootb in .NET, and we have run a spike to confirm that it is strightforward to implement in GOV.UK PaaS


<!-- This is an optional element. Feel free to remove. -->
## Pros and Cons of the Options

### Use Redis as a dedicated shared cache

https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-7.0#distributed-redis-cache

* Good, because it is an industry standard pattern
* Good, because it is supported OOTB by .NET Core
* Good, because Redis is supported as a backing service by GOV.UK PaaS
* Good, because we can maintain horizontal scalability without any load balancer affinity
* Bad, because we would need to add additional infrastructure which risks the delivery deadline

### Use the existing PostgreSQL database

* Good, because the database infrastructure already exists
* Good, because we can maintain horizontal scalability without any load balancer affinity
* Bad, because using PostreSQL as a cache is not supported OOTB in .NET and would need implementing

### Use an in-process in-memory store, with session affinity 

* Good, because it requires no extra infrastructure
* Good, because session data is only ever stored in RAM and never persisted
* Good, because .NET Core supports an in-memory store OOTB
* Bad, because there would be data loss if one of the application instances restarts or during a release


<!-- markdownlint-disable-file MD013 -->
