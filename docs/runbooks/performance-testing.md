---
Last verfied: 2022-07-18
---

# Performance Testing

## Description

Performance testing for the service has been conducted using a custom .NET console app. We chose to use a custom app because the expected dataset of search parameters is known. It is the list of state funded schools in England which is publicly available from the [Get information about Schools](https://get-information-schools.service.gov.uk/) site.

The service downloads and stores the full GIAS dataset in its database nightly. The `GiasPostcodeSearch` console application uses the postcodes from this dataset and runs a search for each. This can be done in parallel for basic performance testing and has the added benefit of testing all possible postcodes to confirm searches using them run without error.

## Results

Date       | Searches | Run Time | Searches Per Second | Average Response Time | Specifications   |
---------- | -------- |----------|---------------------|-----------------------|------------------|
18-07-2022 | 20188    | 5m 26s   | 61                  | 258ms                 | 1x256MB small-13 |
19-10-2022 | 20031    | 8m 48s   | 37                  | 421ms                 | 1x256MB small-13 |

## Runbook

### Scaling

All environments use three nodes for resilience and to confirm that there are no bugs related to an accidental reliance on state in a single node. It is however preferable to run performance testing against a single node to understand what load can be sustained per node.

Change the number of nodes used to one on the target environment by following:

1) Navigate to the Azure Portal.
2) In the left navigation pane, click on "App Services".
3) From the list, select the FaTP Web App.
4) In the left navigation of the web app, under the "Settings" section, select "Scale out (App Service plan)".
5) Use the slider or the specific instance count box to set the number of instances you want for the application to run on.
6) For more advanced scaling, you can also set up autoscaling rules based on metrics like CPU usage or memory. This way, Azure can automatically add or remove instances as needed.
7) Once you've made your selections, click on the "Save" button

### Performance Testing

The `GiasPostcodeSearch` console application is the performance testing application. It loads the full GIAS dataset of elligable schools into memory then uses `Parallel.ForEachAsync` and `HttpClient` to request the search results page using each school's postcode and a subject list relevant to their phase of education.

The application will accept three arguments

* `--url=<FIND_A_TUITION_PARTNER_URL>` (required) - the service url to test against
* `--username=<BASIC_HTTP_AUTH_USERNAME>` (optional) - the basic HTTP authorization username configured for the service url argument
* `--password=<BASIC_HTTP_AUTH_PASSWORD>` (optional) - the basic HTTP authorization password configured for the service url argument

Use the following command to run the application against the chosen environment

```
dotnet run --project GiasPostcodeSearch --url="<FIND_A_TUITION_PARTNER_URL>" --username="<BASIC_HTTP_AUTH_USERNAME>" --password="<BASIC_HTTP_AUTH_PASSWORD>"
```

The console app will print out performance stats for every 500 results similar to the following

```
17500 searches of 20188 run in 284s. 61 searches per second. Average response time 258ms min 128ms (search-results?Data.Postcode=HU9 4JL&Data.Subjects=KeyStage1-English&Data.Subjects=KeyStage2-Maths&Data.Subjects=KeyStage2-Science) max 779ms (search-results?Data.Postcode=WF7 5JB&Data.Subjects=KeyStage1-English&Data.Subjects=KeyStage2-Maths&Data.Subjects=KeyStage2-Science)
```

It will also log an error for any failed searches. The urls logged can be used to rerun the searches used for further debugging and optimization work.