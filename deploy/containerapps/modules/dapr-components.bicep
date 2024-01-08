param environmentName string
param keyVaultName string
param vaultManagedIdentityClientId string
param cosmosDbName string
param cosmosUrl string
param cosmosKey string
param cosmosCollectionName string
param serviceBusConnectionString string

resource environment 'Microsoft.App/managedEnvironments@2022-10-01' existing = {
  name: environmentName

  resource secretstore 'daprComponents@2022-03-01' = {
    name: 'summarizer-secretstore'
    properties: {
      componentType: 'secretstores.azure.keyvault'
      ignoreErrors: false
      version: 'v1'
      initTimeout: '5s'
      metadata: [
        {
          name: 'vaultName'
          value: keyVaultName
        }
        {
          name: 'azureClientId'
          value: vaultManagedIdentityClientId
        }
      ]
      scopes:[
        'summarizer-requests-processor'
      ]
    }
  }

  resource stateStore 'daprComponents@2022-03-01' = {
    name: 'summarizer-statestore'
    properties: {
      componentType: 'state.azure.cosmosdb'
      version: 'v1'
      initTimeout: '5m'
      secrets: [
        {
          name: 'cosmos-key'
          value: cosmosKey
        }
      ]
      metadata: [
        {
          name: 'url'
          value: cosmosUrl
        }
        {
          name: 'masterKey'
          secretRef: 'cosmos-key'
        }
        {
          name: 'database'
          value: cosmosDbName
        }
        {
          name: 'collection'
          value: cosmosCollectionName
        }
        {
          name: 'actorStateStore'
          value: 'true'
        }
      ]
      scopes:[
        'summarizer-api'
      ]
    }
  }

  resource pubsub 'daprComponents@2022-03-01' = {
    name: 'summarizer-pubsub'
    properties: {
      componentType: 'pubsub.azure.servicebus'
      version: 'v1'
      secrets: [
        {
          name: 'service-bus-connection-string'
          value: serviceBusConnectionString
        }
      ]
      metadata: [
        {
          name: 'connectionString'
          secretRef: 'service-bus-connection-string'
        }
      ]
      scopes:[
        'summarizer-requests-processor'
        'summarizer-frontend'
      ]
    }
  }
}
