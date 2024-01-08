# Deploy to Azure with the CLI

### Deploy to Azure with the CLI

1. Ensure you have access to an Azure subscription and the Azure CLI installed
   ```bash
   az login
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
1. Deploy the infrastructure
   ```bash
   az deployment group create --resource-group myrg --template-file ./deploy/containerapps/core.bicep --parameters ./deploy/containerapps/core.bicepparam
   ```
1. Log into Azure Container Registry
   You can get your registry name from your resource group in the Azure Portal
   ```bash
   az acr login --name acrdq74v4fifbfva
   ```
1. Build and push containers (from the root of the repository)
   ```bash
   dotnet publish ./src/frontend --os linux --arch x64 /t:PublishContainer -c Release
   docker tag frontend acrdq74v4fifbfva.azurecr.io/frontend:latest
   docker push acrdq74v4fifbfva.azurecr.io/frontend:latest

   dotnet publish ./src/api --os linux --arch x64 /t:PublishContainer -c Release
   docker tag api acrdq74v4fifbfva.azurecr.io/api:latest
   docker push acrdq74v4fifbfva.azurecr.io/api:latest

   docker build ./src/requests-processor/ -t acrdq74v4fifbfva.azurecr.io/request-processor:latest
   docker push acrdq74v4fifbfva.azurecr.io/request-processor:latest
   ```
1. Deploy the application
   ```bash
   az deployment group create --resource-group myrg --template-file ./deploy/containerapps/app.bicep
   ```

1. Navigate to your frontend URL