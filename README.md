# vitas-dapr-workshop
[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://codespaces.new/arnoldboersma/vitas-dapr-workshop)


## Start app

```bash
dapr run -f .
```

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