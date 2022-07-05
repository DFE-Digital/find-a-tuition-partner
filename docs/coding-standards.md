# Coding Standards

## Introduction

This document collects any project specific coding standards that are not covered by the overall [DfE Technical Guidance](https://technical-guidance.education.gov.uk/). The intention is to automate the checking of these via a linter or similar.

In almost all cases, these standards are the defaults for a fresh installation of Visual Studio and Resharper.

Please be particularly careful around casing. 

## Branches and Commits

* Branches should be named using the format `feature/ntp-xxx-short-description` where NTP-XXX is the Jira ticket reference. Please note: this is all lowercase
* Commit messages should use the format "NTP-XXX: Short description" where NTP-XXX is the Jira ticket reference

## Naming

* Private variables should be prefixed by an underscore e.g. `private string _myString`
* Method names should be descriptive and should avoid `And`
* Unit test names should start `With_` followed by the context in lowecase separated by underscores e.g. `With_no_postcode`

## Spacing

* Remove extra line breaks. There are no cases where more than one line break is needed

## Nesting

* Use the C# 10 file namespace syntax without the nested curly braces

## Imports

* Remove all unused imports highlighted by Visual Studio and Resharper

## Comments

* Only API actions and public methods intended for consumption via a NuGet package should have comment blocks
* Code comments often indicate sections that should be extracted as a method with a descriptive name