---
title: Help
has_children: true
permalink: /lab4
nav_order: 4
---

## Help

### Prepare OpenAI
- Create Azure OpenAI resource
- Open Azure AI studio
Create model with name: `aca-dapr-gpt-35-turbo-01`
![openaimodel](images/openaimodel.png)
- get endpoint and key

### Start app

Open the app on port 11000, 12000 and 13000 the api has an swagger endpoint on /swagger, if you run the application in a githubcodespace the url's can be found as follows:

![Alt text](images/port.png)

> If a port is missing, you can manually add the port to be forwarded.
>
> ![Alt text](images/forewardport.png)

### Debug application

#### Attach Debugger
- First start the application as described in `Start App`
- Open the command palette (Ctrl Shift + P)
- Type `Attach to a .NET`

![Attach to a .NET...](images/attach.png)
- Type `frontend` (or one of the other apps) and select

![select frontend](images/selectprocess.png)
- set a breakpoint!

#### View logs
Output logs from the application and dapr are stored in each application in the `.dapr` folder.

#### Zipkin
![Zipkin](images/zipkin.png)

## Troubleshoot
- Sometimes the Redis container crashes, check the status of the docker container with
```bash
docker ps
```
Or use the Docker plugin.
![container](images/container.png)