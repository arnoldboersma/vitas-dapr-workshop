@description('Provide the default location for resources to .')
param location string = resourceGroup().location
@description('Open AI API key')
@secure()
param openAiApiKey string

@description('Open AI Endpoint')
@secure()
param openAiApiEndpoint string

var uniqueSeed = '${subscription().subscriptionId}-${resourceGroup().name}'
var uniqueSuffix = uniqueString(uniqueSeed)

module log './modules/log.bicep' = {
  name: '${deployment().name}-log'
  params: {
    location: location
    uniqueSuffix: uniqueSuffix
  }
}
module azureservices './modules/azure-services.bicep' = {
  name: '${deployment().name}-azureservices'
  params: {
    location: location
    uniqueSuffix: uniqueSuffix
    openAiApiEndpoint: openAiApiEndpoint
    openAiApiKey: openAiApiKey
  }
}

module aca './modules/cae.bicep' = {
  name: '${deployment().name}-cae'
  params: {
    location: location
    logAnalyticsWorkspaceId: log.outputs.id
    applicationInsightsConnectionString: log.outputs.connectionString
    uniqueSuffix: uniqueSuffix
  }
}

module daprcomponents './modules/dapr-components.bicep' = {
  name: '${deployment().name}-daprcomponents'
  params: {
    environmentName: aca.outputs.environmentName
    keyVaultName: azureservices.outputs.keyVaultName
    vaultManagedIdentityClientId: azureservices.outputs.vaultManagedIdentityClientId
    cosmosCollectionName: azureservices.outputs.cosmosCollectionName
    cosmosDbName: azureservices.outputs.cosmosDbName
    cosmosUrl: azureservices.outputs.cosmosUrl
    cosmosKey: azureservices.outputs.cosmosKey
    serviceBusConnectionString: azureservices.outputs.serviceBusConnectionString
  }
}

module acr './modules/acr.bicep' = {
  name: '${deployment().name}-acr'
  params: {
    location: location
    uniqueSuffix: uniqueSuffix
    managedIdentityPrincipalId: azureservices.outputs.managedIdentityPrincipalId
  }
}
