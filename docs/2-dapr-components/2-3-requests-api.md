---
title: Requests API
parent: Implement Dapr Components
has_children: false
permalink: /lab2/requests-api
nav_order: 2
---

# Requests API

{: .no_toc }

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
- TOC
{:toc}
</details>

## Add Request API to `dapr.yaml`

As we will be having many dapr applications, let's leverage the [multi-run](https://docs.dapr.io/developing-applications/local-development/multi-app-dapr-run/) feature of Dapr to run all the applications at once. For now, let's add the request api only to the `dapr.yaml` file.

* Open the `dapr.yaml` file in the `/` folder and add a new node for the application including the environment variables.

<details markdown="block">
  <summary>
    Toggle solution
  </summary>

```yaml
  - appID: summarizer-api
    appDirPath: ./src/api/
    appPort: 12000
    command: ["dotnet", "watch"]
    env:
      STATE_STORE_NAME: "summarizer-statestore"
      STATE_STORE_QUERY_INDEX_NAME: "orgIndx"
      DOTNET_URLS: "http://*:12000"
      ASPNETCORE_ENVIRONMENT: "Development"
      DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER: "true"
```
</details>

> Note: The `appID` is used to identify the application in the Dapr runtime. The `appDirPath` is the path to the application folder. The `appPort` is the port used by the application. The `command` is the command used to start the application. The `env` is the environment variables used by the application.

## Create State Store yaml

From the previous definition of the environment variables of our application, we can see that we will be using a state store. Let's create the yaml file for the state store.

* Create a new file named `summarizer-statestore.yaml` in the `/dapr/local/components` folder, add a state store component called 'summarizer-statestore' and configure it to use Redis. Query indexes should also be configured to allow for fast retrieval of data (url, url_hashed and id).

<details markdown="block">
  <summary>
    Toggle solution
  </summary>

```yaml
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: summarizer-statestore
spec:
  type: state.redis
  version: v1
  metadata:
  - name: redisHost
    value: localhost:6379
  - name: redisPassword
    value: ""
  - name: queryIndexes
    value: |
      [
        {
          "name": "orgIndx",
          "indexes": [
            {
              "key": "id",
              "type": "TEXT"
            },
            {
              "key": "url",
              "type": "TEXT"
            },
            {
              "key": "url_hashed",
              "type": "TEXT"
            }
          ]
        }
      ]
```
</details>

> Note: The `name` is the name of the component. The `type` is the type of the component. The `version` is the version of the component. The `metadata` is the configuration of the component. We also added a query index to the state store to be able to query the state store by url. url_hashed is the hashed version of the url to ease the search query and avoid any special characters issues.

## Request API Overview

1. Open the `Program.cs` file in the `/src/api` folder, and notice the DaprClient object that is used to interact with the Dapr runtime.

```csharp
builder.Services.AddDaprClient(client =>
{
    client.UseJsonSerializationOptions(new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    });
});
```
2. We also have several endpoints defined to manage the requests lifecycle :
    * `app.MapGet("/requests"...` : Get all the requests
    * `app.MapPost("/search-requests-by-url"...` : Search a request with specific link
    * `app.MapPost("/requests"...` : Create a request

## Implementing state methods

Let's implement the state methods.

1. Open the `SummarizeRequestService.cs` file in the `/src/api/Services` folder, managing the state store.

2.  Fill the `GetSummaryRequestsAsync` method to get all the requests from the state store. The method should return a list of `SummaryRequest` objects.

<details markdown="block">
  <summary>
    Toggle solution
  </summary>

{% raw %}
```csharp
        var query = """
        {
            "page": {
                "limit": 100
            }
        }
        """;

        // Get State
        Dictionary<string, string> metadata = new() { { "contentType", "application/json" }, { "queryIndexName", this.appSettings.StateStoreQueryIndexName } };
        var queryResult = await daprClient.QueryStateAsync<SummaryRequest>(appSettings.StateStoreName, query, cancellationToken: ct, metadata: metadata);
        return queryResult.Results.Select(s => s.Data).ToList();
```
{% endraw %}
</details>

2.  Fill the `CreateSummaryRequestAsync` method to save a requests to the state store. The method should return the created `SummaryRequest` object.

<details markdown="block">
  <summary>
    Toggle solution
  </summary>

{% raw %}
```csharp
        Dictionary<string, string> metadata = new() { { "contentType", "application/json" } };
        var request = new SummaryRequest
        {
            Id = Guid.NewGuid(),
            Email = newSummarizeRequest.Email,
            Url = newSummarizeRequest.Url,
            UrlHashed = GetHashedUrl(newSummarizeRequest.Url),
            Summary = newSummarizeRequest.Summary,
        };

        // Save State
        await this.daprClient.SaveStateAsync(appSettings.StateStoreName, request.Id.ToString(), request, metadata: metadata, cancellationToken: ct);

        return request;
```
{% endraw %}
</details>

3.  Fill the `SearchSummaryRequestByUrlAsync` method to get a request from the state store using a specific property and value. The method should return a `SummaryRequest` object or None.

<details markdown="block">
  <summary>
    Toggle solution
  </summary>

{% raw %}
```csharp
        var query = $$"""
        {
            "page": {
                "limit": 100
            },
            "filter": {
                "EQ": {
                    "url_hashed": "{{GetHashedUrl(searcRequest.Url)}}"
                }
            }
        }
        """;

        // Search Summary Request by URL
        Dictionary<string, string> metadata = new() { { "contentType", "application/json" }, { "queryIndexName", this.appSettings.StateStoreQueryIndexName } };
        var queryResult = await daprClient.QueryStateAsync<SummaryRequest>(appSettings.StateStoreName, query, cancellationToken: ct, metadata: metadata);
        return queryResult.Results.Select(s => s.Data).FirstOrDefault();
```
{% endraw %}
</details>


## Validate that the request is stored in the state store

1. Execute dapr run multi run command to start the application

    ```bash
    dapr run -f .
    ```
2. Open the swagger endpoint on /swagger

3. Check that the request is stored in the state store