---
title: Requests Processor
parent: Implement Dapr Components
has_children: false
permalink: /lab2/requests-processor
nav_order: 4
---

# Requests Processor

{: .no_toc }

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
- TOC
{:toc}
</details>

## Add Request Processor to `dapr.yaml`

As we will be having many dapr applications, let's leverage the [multi-run](https://docs.dapr.io/developing-applications/local-development/multi-app-dapr-run/) feature of Dapr to run all the applications at once. So let's add Requests Processor App to the `dapr.yaml` file.

* Open the `dapr.yaml` file in the `/` folder and add a new node for the application including the environment variables.

<details markdown="block">
  <summary>
    Toggle solution
  </summary>

```yaml
  - appID: summarizer-requests-processor
    appDirPath: ./src/requests-processor/
    appPort: 13000
    command: ["uvicorn", "app:app", "--host", "0.0.0.0", "--port", "13000"]
    env:
      SECRET_STORE_NAME: "summarizer-secretstore"
      PUBSUB_REQUESTS_NAME: "summarizer-pubsub"
      PUBSUB_REQUESTS_TOPIC: "link-to-summarize"
      REQUESTS_API_APP_ID: "summarizer-api"
      REQUESTS_API_SEARCH_ENDPOINT: "search-requests-by-url"
      REQUESTS_API_CREATE_ENDPOINT: "requests"
      OPENAI_API_VERSION: "2022-12-01"
      OPENAI_API_DEPLOYMENT_NAME: "aca-dapr-gpt-35-turbo-01"
      APP_PORT: 13000
```
</details>

> Note: The `appID` is used to identify the application in the Dapr runtime. The `appDirPath` is the path to the application folder. The `appPort` is the port used by the application. The `command` is the command used to start the application. The `env` is the environment variables used by the application.

> Note: PubSub are already created, so we will be reusing the existing definition created before.

## Request Processor Overview

1. Open the `app.py` file in the `/src/request-processor` folder, and notice the subscribe method being used as en entry point

```python
# Subscribe to a new request from the pub/sub component
async def link_to_summarize(event : SummarizeRequestCloudEvent):
```

2. Open the `request_handler.py` file in the `/src/request-processor` folder, and notice how the request is being processed .

## Validate the overall processes

1. Execute dapr run multi run command to start the application

    ```bash
    dapr run -f .
    ```
2. Open the blazor application, create a new request using any email and link

3. Check that the request is stored in redis and refresh the blazor page to see the summary of the request.

{: .no_toc }
## Congratulations !

You've been able to implement Dapr Components to an existing application to avoid strong coupling between application dependancies. In the next chapter, we will see how to use Dapr to deploy the application to Azure Container Apps.