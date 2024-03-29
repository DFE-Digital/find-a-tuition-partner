## Context

<!-- Why are you making this change? What might surprise someone about it? -->

## Changes proposed in this pull request

<!-- If there are UI changes, please include Before and After screenshots. -->

## Guidance to review

<!-- How could someone else check this work? Which parts do you want more feedback on? -->

## Link to Jira ticket

<!-- https://dfedigital.atlassian.net/browse/NTP-123 -->

## Things to check

- [ ] Title is in the format "NTP-XXX: Short description" where NTP-XXX is the Jira ticket reference
- [ ] Code and tests follow the [coding standards](/docs/coding-standards.md)
- [ ] `dotnet format` has been run in the repository root
- [ ] Test coverage of new code is at least 80%
- [ ] All UI related acceptance criteria specified in the ticket have been added to the relevant [cypress test feature file(s)](/UI/cypress/e2e/)
- [ ] Unless explicitly mentioned in the ticket, all UI work should have been tested as working on desktop and mobile resolutions with JavaScript on and off
- [ ] Database migrations can be applied to the current codebase in main without causing exceptions
- [ ] Debug logging has been added after all logic decision points
- [ ] All [automated testing](/README.md#testing) including accessibility and security has been run against your code
- [ ] **PR deployment has been signed off by wider team**