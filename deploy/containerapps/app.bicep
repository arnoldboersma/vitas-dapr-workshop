@description('Provide the default location for resources to .')
param location string = resourceGroup().location

var uniqueSeed = '${subscription().subscriptionId}-${resourceGroup().name}'
var uniqueSuffix = uniqueString(uniqueSeed)

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: 'appi-${uniqueSuffix}'
}

resource environment 'Microsoft.App/managedEnvironments@2022-10-01' existing = {
  name: 'cae-${uniqueSuffix}'
}

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: 'mi-${uniqueSuffix}'
  location: location
}

resource keyVaultManagedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: 'mi-kv-${uniqueSuffix}'
  location: location
}

var frontendSettings = [
  {
    name: 'PUBSUB_REQUESTS_NAME'
    value: 'summarizer-pubsub'
  }
  {
    name: 'PUBSUB_REQUESTS_TOPIC'
    value: 'link-to-summarize'
  }
  {
    name: 'REQUESTS_API_APP_ID'
    value: 'summarizer-api'
  }
  {
    name: 'REQUESTS_API_ENDPOINT'
    value: 'requests'
  }
]

var apiSettings = [
  {
    name: 'STATE_STORE_NAME'
    value: 'summarizer-statestore'
  }
  {
    name: 'STATE_STORE_QUERY_INDEX_NAME'
    value: 'orgIndx'
  }
]

var workerSettings = [
  {
    name: 'SECRET_STORE_NAME'
    value: 'summarizer-secretstore'
  }
  {
    name: 'PUBSUB_REQUESTS_NAME'
    value: 'summarizer-pubsub'
  }
  {
    name: 'PUBSUB_REQUESTS_TOPIC'
    value: 'link-to-summarize'
  }
  {
    name: 'REQUESTS_API_APP_ID'
    value: 'summarizer-api'
  }
  {
    name: 'REQUESTS_API_SEARCH_ENDPOINT'
    value: 'search-requests-by-url'
  }
  {
    name: 'REQUESTS_API_CREATE_ENDPOINT'
    value: 'requests'
  }
  {
    name: 'OPENAI_API_VERSION'
    value: '2022-12-01'
  }
  {
    name: 'OPENAI_API_DEPLOYMENT_NAME'
    value: 'aca-dapr-gpt-35-turbo-01'
  }
  {
    name: 'APP_PORT'
    value: '8080'
  }
]

module frontend './modules/container-app.bicep' = {
  name: '${deployment().name}-frontend'
  params: {
    location: location
    applicationInsightsConnectionString: applicationInsights.properties.ConnectionString
    appName: 'summarizer-frontend'
    azureContainerRegistryName: 'acr${uniqueSuffix}'
    image: 'acr${uniqueSuffix}.azurecr.io/frontend:latest'
    external: true
    managedEnvironmentId: environment.id
    containerRegistryUserAssignedIdentityId: managedIdentity.id
    appSettings: frontendSettings
    keyVaultUserAssignedIdentityId: keyVaultManagedIdentity.id
  }
}

module api './modules/container-app.bicep' = {
  name: '${deployment().name}-api'
  params: {
    location: location
    applicationInsightsConnectionString: applicationInsights.properties.ConnectionString
    appName: 'summarizer-api'
    azureContainerRegistryName: 'acr${uniqueSuffix}'
    image: 'acr${uniqueSuffix}.azurecr.io/api:latest'
    external: true
    managedEnvironmentId: environment.id
    containerRegistryUserAssignedIdentityId: managedIdentity.id
    appSettings: apiSettings
    keyVaultUserAssignedIdentityId: keyVaultManagedIdentity.id
  }
}

module summarizer './modules/container-app.bicep' = {
  name: '${deployment().name}-summarizer'
  params: {
    location: location
    applicationInsightsConnectionString: applicationInsights.properties.ConnectionString
    appName: 'summarizer-requests-processor'
    azureContainerRegistryName: 'acr${uniqueSuffix}'
    image: 'acr${uniqueSuffix}.azurecr.io/request-processor:latest'
    external: true
    managedEnvironmentId: environment.id
    containerRegistryUserAssignedIdentityId: managedIdentity.id
    appSettings: workerSettings
    keyVaultUserAssignedIdentityId: keyVaultManagedIdentity.id
  }
}

