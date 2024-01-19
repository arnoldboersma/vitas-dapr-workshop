# vitas-dapr-workshop
[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://codespaces.new/arnoldboersma/vitas-dapr-workshop)

[Hands-on-guide :](https://arnoldboersma.github.io/vitas-dapr-workshop/)

Welcome to the **Vitas Dapr, Azure Container Apps Workshop**. You'll be experimenting with Azure Serverless services in multiple labs to achieve a real world scenario. Don't worry, even if the challenges will increase in difficulty, this is a step by step lab, you will be guided through the whole process.

During this workshop you will have the instructions to complete each steps. It is recommended to search for the answers in provided resources and links before looking at the solutions placed under the 'Toggle solution' panel.

## Summarizer

**Summarizer** is an application designed to demonstrates how to build a, intelligent cloud native application using Dapr, Containers Apps and Open AI. It is composed of multiple microservices.

![SummarizeArchitecture](docs/images/summarizer-dapr-aca.drawio.png)

**Summarizer Blazor App (Front-end)** : A Blazor SSR application that allows to browse all summaries and eventually queue new links to be summarized.

**Requests API (ASP.NET Core)** : ASP.NET Core minimal API leveraging Dapr state management to store / get all requests of summaries. It allows to track and reuse previous summaries eventually generated.

**Requests Processor (Python)** : A Python application that allows to process summary requests in queue. If no summary has already being provided, it will prompt Azure Open AI to get a new summary. In any case, requests will be tracked using the requests API at the end of the process.
