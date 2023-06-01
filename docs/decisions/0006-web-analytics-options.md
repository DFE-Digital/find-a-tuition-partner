---
# These are optional elements. Feel free to remove any of them.
status: accepted
date: 2022-12-07
deciders: NTP developers
---

# A solution for non cookie-based (therefore non opt-in) anonymised web analytics

## Context and Problem Statement

The find a tuition partner service uses Google Analytics (GA) injected via Google Tag Manager (GTM). GTM has been used to send events not natively tracked by GA specifically when the user attempts to contact a TP or visit their website.

GA and GTM have the big advantage of being widely understood as well as relatively easy to integrate with and visualise metrics. Their disadvantage in a government context is the need for users to opt into metrics gathering via GA and GTM that uses cookies. This significantly reduces the accuracy of the gathered metrics.

It has also proven difficult to extract the search parameters used from the query string in GA. This has hampered efforts to identify which schools are using the service.

<!-- This is an optional element. Feel free to remove. -->

## Decision Drivers

- Opt-in nature of cookie based GTM/GA significantly reduces the accuracy of the gathered metrics.
- Difficult to extract the search parameters used from the query string in GA.

## Considered Options

- Extracting analytics from logit.io logs
- Reuse the `dfe-analytics` pattern

## Decision Outcome

Chosen option: "Reuse the `dfe-analytics` pattern", because
it is an existing pattern already used with DfE and allows us to draw on existing knowledge and Dataforms patterns within DfE.

## Pros and Cons of the Options

### Extracting analytics from logit.io logs

The find a tuition partner service uses logit.io as a log aggregator. This is an ELK (Elastic, Logstash, Kibana) stack as a service product. The intended usage of this service was log analysis and exception alerting. All events from the service are logged and as a result, it can also become a source of metrics data. Postcodes used to search for TPs in the service are extracted from Logit.io: Observability Built On Hosted OpenSearch, Jaeger & Grafana .

Every event is logged consistently regardless of client side metrics opt in. This means that we can rely on the accuracy of the usage counts for service functionality. This is a significant advantage over GA. The opensearch and grafana dashboards provided by Logit.io: Observability Built On Hosted OpenSearch, Jaeger & Grafana provide significant power to analyse and visualise metrics data. Skills in this area are rare however and the existing retention period of 14 days make retrospective analysis of metrics impossible. The retention period could be extended to up to a year if agreed and budgeted for.

- Good, because we are already collecting the data
- Bad, because skills in this area are rare
- Bad, because existing retention period of 14 days make retrospective analysis of metrics impossible

### Reuse the `dfe-analytics` pattern

Teacher Services have a mature metrics solution based on streaming event data to Google BigQuery tables. They have created the dfe-analytics Ruby gem which uses the Google Cloud streaming API (specifically the legacy version) to stream events into a predefined, generic events table. Consuming and configuring this gem is a development task.

This generic events table is transformed using dataform into domain specific datasets (tables) that are more easily consumed in Google Data Studio. The dfe-analytics-dataform repo can be added as a dependency within your dataform project to provide SQL functions and table definitions for the generic events table. This is intended to make transforming the data in the events table easier. Managing the dataform task requires SQL skills and is usually handled by a Performance Analyst or Business Analyst. It can also fall under the developer remit.

A team has ported the Ruby gem to dotnet, starting with capturing web requests only rather than the full database and model changes. This is however sufficient for our needs. The port is currently an internal project and won’t be made publicly available as a NuGet package in the short term. We may be able to reference the project directly to prevent duplication of effort

- Good, because it is an established pattern withing DfE
- Good, because it stores data for as long as we require
- Bad, because the `dfe-analytics-dotnet` library is quite immature

## More Information

- https://github.com/DFE-Digital/dfe-analytics
- https://github.com/DFE-Digital/dfe-analytics-dotnet

<!-- markdownlint-disable-file MD013 -->
