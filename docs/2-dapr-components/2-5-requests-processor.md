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

## Create Secretstore yaml

Let's create the yaml file for the Secretstore component.

* Create a new file named `summarizer-pubsub` in the `/dapr/local/components` folder using local files as the secretstore component.

<details markdown="block">
  <summary>
    Toggle solution
  </summary>

```yaml
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: summarizer-secretstore
spec:
  type: secretstores.local.file
  version: v1
  metadata:
  - name: secretsFile
    value: /workspaces/vitas-dapr-workshop/dapr/summarizer-secrets.json
  - name: nestedSeparator
    value: ":"
```
</details>

> Note: The `name` is the name of the component. The `type` is the type of the component. The `version` is the version of the component. The `metadata` is the configuration of the component.


## Request Processor Overview

1. Open the `app.py` file in the `/src/request-processor` folder, and notice the subscribe method being used as en entry point

```python
# Subscribe to a new request from the pub/sub component
async def link_to_summarize(event : SummarizeRequestCloudEvent):
```

2. Open the `request_handler.py` file in the `/src/request-processor` folder, and notice how the request is being processed.

3. Find method `__get_summary` and change the implementation to

```python
    async def __get_summary(self, url):
        logging.info(f"URL: {url}")

        client = AzureOpenAI(
            api_key=self.settings.api_key,
            api_version=self.settings.open_api_version,
            azure_endpoint = self.settings.open_api_endpoint
            )

        try:
            response = client.completions.create(model=self.settings.open_api_deployment_name,
            prompt=f"Summarize the article {url} in english in less than two paragraphs without adding new information. When the summary seems too short to make at least one paragraph, answer that you can't summarize a text that is too short",
            temperature=0.9,
            max_tokens=200,
            top_p=1,
            frequency_penalty=0,
            presence_penalty=0.6)
            logging.info(response.choices[0].text)
            return response.choices[0].text

        except Exception as e:
            logging.error(e)
            return "Unable to summarize this article."
```

## Validate the overall processes

1. Execute dapr run multi run command to start the application

    ```bash
    dapr run -f .
    ```
2. Open the blazor application, create a new request using any email and link

3. Check that the request is stored in redis and refresh the blazor page to see the summary of the request.

4. BONUS: Change OpenAI parameters

{: .no_toc }
## Congratulations !

You've been able to implement Dapr Components to an existing application to avoid strong coupling between application dependancies. In the next chapter, we will see how to use Dapr to deploy the application to Azure Container Apps.