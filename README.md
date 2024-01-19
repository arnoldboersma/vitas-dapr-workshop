# vitas-dapr-workshop
[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://codespaces.new/arnoldboersma/vitas-dapr-workshop)

[Hands-on-guide :](https://arnoldboersma.github.io/vitas-dapr-workshop/)

## Prepare OpenAI
- TODO Create Azure OpenAI resource
- Open Azure AI studio
Create model with name: `aca-dapr-gpt-35-turbo-01`
![openaimodel](docs/images/openaimodel.png)
- get endpoint and key
- copy .dapr/summarizer-secrets-sample.json to .dapr/summarizer-secrets.json and fill in the secret values

## Start app

```bash
dapr run -f .
```

Open the app on port 11000, 12000 and 13000 the api has an swagger endpoint on /swagger, if you run the application in a githubcodespace the url's can be found as follows:

![Alt text](docs/images/port.png)

> If a port is missing, you can manually add the port to be forwarded.
>
> ![Alt text](docs/images/forewardport.png)

## Debug application

### Attach Debugger
- First start the application as described in `Start App`
- Open the command palette (Ctrl Shift + P)
- Type `Attach to a .NET`

![Attach to a .NET...](docs/images/attach.png)
- Type `frontend` (or one of the other apps) and select

![select frontend](docs/images/selectprocess.png)
- set a breakpoint!

### View logs
Output logs from the application and dapr are stored in each application in the `.dapr` folder.

### Zipkin
![Zipkin](docs/images/zipkin.png)

## Deploy to azure
[Deploy to Azure with the CLI](./deploy//containerapps/README.md)

## Troubleshoot
- Sometimes the Redis container crashes, check the status of the docker container with
```bash
docker ps
```
Or use the Docker plugin.
![container](docs/images/container.png)