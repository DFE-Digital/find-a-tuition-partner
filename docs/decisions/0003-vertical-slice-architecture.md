---
# These are optional elements. Feel free to remove any of them.
status: rejected
date: 2022-06-22
deciders: NTP developers
---

# Organise code in vertical slices

## Context and Problem Statement

A common code organisation architecture is the "clean architecture" (otherwise known as Onion architecture, or Port and Adapters)
which organises classes into various modules (projects in c#) based upon their classification. Common groupings are Framework -
containing code relevant to the particular UI framework in use; Infrastructure - containing adapters for databases, message queues, etc.;
Application - containing use cases / features; and Domain - containing the enterprise business rules.

Whilst this separation can be a useful step to create a decoupled codebase, it has a number of negatives in the vast majority of implementations:

- Code for a single feature is almost always split across every layer
- Code is physically organised by layer, which enforces separating logically coupled code from into many places in the codebase
- The codebase looks like a collection of Controllers, Handlers, Exceptions and other technical categories, rather than Features and Capabilities
- The layering approach means a single technical decision (e.g. "use an ORM for data access") is used for all features, regardless of applicability

## Decision Drivers

- The intention to enhance a notion of the project's features
- The developer experience when working with the codebase

## Decision Outcome

Chosen option: "organise code in vertical slices", because it promotes encapsulating the feature into a single area.

### Organise code in vertical slices

- Good, because code for a single feature is easy to find in the codebase
- Good, because code for a single feature cannot impact any other feature
- Good, because features can use the most appropriate solutions for their individual requirements
- Neutral, because Clean code / Onion layers can still be used in a slice if helpful
- Bad, it is a less common organisation with which developers may be unfamiliar

### Organise code in traditional onion architecture

- Good, it is more familiar
- Bad, because it os more complex
- Bad, because feature related code is spread around the solution

## More Information

https://jimmybogard.com/vertical-slice-architecture/

### Reason for rejection

In a development team meeting on 16th Feb 2023, it was decided that we would continue with the clean architecture approach:

- Most of the codebase is already using a clean architecture approach
- The development team are more familiar with a clean architectural approach
- The limited lifespan of the service means that long term supportability is less of an issue

<!-- markdownlint-disable-file MD013 -->
