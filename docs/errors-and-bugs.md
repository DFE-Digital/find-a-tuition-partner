# Errors and Bugs

## Introduction

This document details the ways by which errors and bugs can be surfaced to the team and subsequently resolved. It covers prioritization of bugs and estimated resolution times.

## Priority

We use four priority levels from most to least impactful:

* P1 - Widespread user impact preventing completion of at least one user journey. Usually connected to an outage
* P2 - User visible issue affecting at least one user journey. Either does not prevent journey or can be worked around
* P3 - Minor user visible issue that does not directly affect a user journey such as a formatting issue
* P4 - Issue not visible to the user for example an exception in the logs

## Triage Process

An issue raised by one of the sources below should be triaged to determine the priority. The triage process should be undertaken by one or more of the following diciplines; Business Analyst, Delivery Manager, Product Manager plus a member of the development team. That group should use the impacts associated with the priority list to assign the issue the correct priority.

## Sources

There are three ways the team can be notified of errors and bugs. In each case the issue should be triaged quickly and either created as a new bug ticket in Jira with a priority or identified as an existing known issue.

### Via Service Desk

A user contacting the service desk will result in the creation of a Zendesk ticket. That ticket will be triaged to the shared inbox. The ticket should be linked to any resulting bug ticket in Jira and both should be resolved together.

### Via Alerts Posted to Slack

Warning and above log entries generate alerts from Azure App Insights. These alerts are posted to the following Slack channels and via email:

* #ntp-find-a-tuition-partner-alerts (via exception_alerts.yaml) - alerts from the *Production* environment. **Should be investigated and triaged immediately**
* #ntp-dev-team (via exception_alerts_non_production.yaml) - alerts from the non production environment

### Via Testing

Each ticket is signed off by the wider team by testing the functionality. Issues not preventing release of the new functionality can be recorded as low priority bugs in the backlog.

## Prioritisation and Resolution Times

* P1 - Should be actioned immediately and fixed within 24 hours where possible
* P2 - Should be added to the current sprint and resolved within 7 days
* P3 - Add to backlog and include in next sprint
* P4 - Add to backlog and resolve when capacity is available.

## Notifications

The service desk should be notified of P1 and P2 issues so that they can inform and users that contact them that they are aware of the issue and that it is being resolved.