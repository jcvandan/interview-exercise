# OpenMoney Interview Exercise

## Pre-requisites

- .NET 5 SDK installed on your machine
- A .NET IDE of your choice

## Introduction

A developer on your team, Baldrick, has just quit with no notice period. And good riddance because he was overpaid and made a mess of the codebase...Unfortunately this means you have to pick up the ticket he was working on before he left. As expected, he's done a bad job. The code is a mess, there are bugs to fix, and it's missing an important feature. However, the CEO has an expensive marketing campaign that's going out tonight and you finish work in an hour. Whatever you manage in the next 60 minutes will just have to be enough (the CEO shouldn't have really paid for the marketing campaign yet anyway).

The feature you're looking at is a Mortgage and Home Insurance quote provider service. The actual quotes themselves are coming back from third party integrations, the interfaces for the third parties have been provided by our partners so we can't change these.

Below are a few tasks to get through so we can ship the new feature! Also as Baldrick was a complete incompetent, the code he's written is ugly and reads like The Daily Star, not the well written prose we expect. While working through the following tasks please tidy up the code as you go.

## Acceptance Criteria

This is the original acceptance criteria for the 2 integrations:

- Mortgage
  - Call our partner via the `IThirdPartyMortgageApi` interface
  - Select and return the quote with the lowest monthly payment
  - If the Loan to Value % of the requested mortgage is lower than 10%, don't fetch a quote as it the request is not eligible
- Home
  - Call our partner via the `IThirdPartyHomeInsuranceApi` interface
  - Select and return the quote with the lowest monthly payment
  - If the requested house value is greater than 10 million, don't fetch a quote as it the request is not eligible

## Summary

- Work through the below tasks
- Tidy up the code as you go
- Do as much as you can in an hour
- Submit a PR for us to review

## Tasks

### Task 1 - Bug

QA have reported that some strange negative monthly payment values have been coming back from the mortgage provider. All the unit tests are passing but maybe the unit tests are as useless as some of the production code. See if you can figure out what the issue is.

### Task 2 - Performance

QA have also noticed that it seems to be taking longer than expected. As the 3rd party integrations are HTTP based, we can't do anything about how long the requests take to complete, but can you identify anything within the existing code that might improve this area?

### Task 3 - Missing tests

As usual, Baldrick made a token effort to write some unit tests but has made some glaring omissions. Try and identify some areas that are missing test coverage and add them in.

### Task 4 - Error handling

Our customers hate unhandled exceptions, as should you, so make our new feature handle errors gracefully. It would be nice if we could let our consumers know which quote failed, and still return quotes that succeeded when another has failed.

### Bonus Task

The `MortgageClient` and `HomeInsuranceClient` classes share quite a few similarities, is there anything you could do to reduce some duplication between these 2 classes?
