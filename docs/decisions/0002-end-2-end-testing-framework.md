---
status: accepted
date: 2022-06-17
---

# 2. End to End Testing Framework

## Context and Problem Statement

The delivery team does not have QA resource available and as a result, automated end to end testing is an important requirement over an above the general guidance to include automated testing in any project.

<!-- This is an optional element. Feel free to remove. -->

## Decision Drivers

- Development environment set up complexity
- Team skill set
- Support for accessing sites protected via basic HTTP authentication
- Complexity of writing tests
- Speed of test execution
- Ability to run headless
- Ability to integrate into a GitHub action
- Test execution reporting

## Considered Options

- SpecFlow
- Cypress

## Decision Outcome

Chosen option: "Cypress", because it is open source, well documented, was straightforward to set up and integrates well with GitHub actions. It reports test results well and is extremely fast to run. This outweighs the fact that it is primarily a JavaScript tool used by a .NET team.

## Pros and Cons of the Options

### SpecFlow

- Good, because it is written in .NET
- Good, because is it familiar to the majority of .NET developers
- Good, because it can be run from within Visual Studio
- Bad, because it relies on Selenium web drivers which can be fiddly to maintain the right versions of
- Bad, because the project file structure is complex
- Bad, because dependency injection for steps and features is minimal
- Bad, because it is slower to execute than Cypress

### Cypress

- Good, because the documentation is excellent
- Good, because it is heavily used and supported within the NodeJS community
- Good, because it proved simple to set up by a primarily .NET developer
- Good, because it integrates well and simply with GitHub actions
- Good, because it proved significantly faster to execute tests than SpecFlow
- Good, because it reports test results simply and effectively and will produce a video of the test run
- Good, because easy to config and run in different Environment like DEV,QA,PROD
- Good, because it runs the test without need to of selenium drivers in developers laptop
- Neutral, because it is a command line tool which requires NodeJS installed and that can put of .NET devs who prefer all tooling available in Visual Studio
