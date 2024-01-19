# Deploy to Azure with the CLI

### Deploy to Azure with the CLI

1. Ensure you have access to an Azure subscription and the Azure CLI installed
   ```bash
   az login --use-device-code
   az upgrade
   az account set --subscription "My Subscription"
   ```
1. Ensure you have access to an Azure subscription and the Azure CLI installed
   ```bash
   az extension add --name containerapp --upgrade
   ```
1. Clone this repository
   ```bash
   git clone https://github.com/arnoldboersma/vitas-dapr-workshop.git
   cd vitas-dapr-workshop
   ```
1. Create your own resourcegroup
1. Modify core.bicepparam with the secrets
1. Deploy the infrastructure
   ```bash
   az deployment group create --resource-group <your resourcegroup name> --template-file ./deploy/containerapps/core.bicep --parameters ./deploy/containerapps/core.bicepparam
   ```
Give yourself acr push role for Container Registry in IAM

1. Log into Azure Container Registry
   You can get your registry name from your resource group in the Azure Portal
   ```bash
   az acr login --name <name container instance>
   ```
1. Build and push containers (from the root of the repository)
   ```bash
   dotnet publish ./src/frontend --os linux --arch x64 /t:PublishContainer -c Release
   docker tag frontend <name container instance>.azurecr.io/frontend:latest
   docker push <name container instance>.azurecr.io/frontend:latest

   dotnet publish ./src/api --os linux --arch x64 /t:PublishContainer -c Release
   docker tag api <name container instance>.azurecr.io/api:latest
   docker push <name container instance>.azurecr.io/api:latest

   docker build ./src/requests-processor/ -t <name container instance>.azurecr.io/request-processor:latest
   docker push <name container instance>.azurecr.io/request-processor:latest
   ```
1. Deploy the application
   ```bash
   az deployment group create --resource-group <your own resourcegroup> --template-file ./deploy/containerapps/app.bicep
   ```

1. Navigate to your frontend URL