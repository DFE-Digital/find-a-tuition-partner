---
status: accepted
date: 2022-07-05
deciders: NTP developers
---
# Adopt Gherkin BDD Syntax for End to End Testing

## Context and Problem Statement

The feature tickets for the service contain acceptance criteria writen in the Gherkin Given, When, Then style. The cypress end to end tests in the service do not use these Gherkins and instead are written procedurally with an overall descriptive name. This requires a translation effort on the part of the developer and extra explanation of how the test works to prove that it covers the AC.

## Decision Outcome

Chosen option: "Adopt Gherkin BDD Syntax", because it is a common language between Business Analysts and Developers.

### Positive Consequences

* A link to the feature file(s) can be added to the completed ticket to prove all ACs have automated end to end tests
* Can build towards a set of steps that can be used in the ACs ultimately resulting in copying the ACs directly from the ticket into the feature file(s)
* The full functionality of the service is described by the feature files

### Negative Consequences

* Dicipline and attention to detail is required to manage the location and library of shared and feature specific steps
* Cypress does not support the Gherkin syntax natively. Support is provided by the [cypress cucumber preprocessor](https://github.com/badeball/cypress-cucumber-preprocessor)

## More Information

* [Gherkin syntax](https://cucumber.io/docs/gherkin/reference/)
* [Cypress cucumber preprocessor documentation](https://github.com/badeball/cypress-cucumber-preprocessor/tree/master/docs)
