param location string = resourceGroup().location
@minLength(13)
@maxLength(13)
param uniqueSuffix string
@description('Open AI API key')
@secure()
param openAiApiKey string

@description('Open AI Endpoint')
@secure()
param openAiApiEndpoint string

var managedIdentityName = 'mi-${uniqueSuffix}'

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: managedIdentityName
  location: location
}

module keyVault 'keyvault.bicep' = {
  name: '${deployment().name}-infra-keyvault'
  params: {
    location: location
    keyVaultName: 'kv-${uniqueSuffix}'
    keyVaultUserAssignedIdentityName:'mi-kv-${uniqueSuffix}'
    openAiApiEndpoint: openAiApiEndpoint
    openAiApiKey: openAiApiKey
  }
}

module cosmos 'cosmos-db.bicep' = {
  name: '${deployment().name}-infra-cosmos-db'
  params: {
    location: location
    cosmosAccountName: 'cosmos-${uniqueSuffix}'
    cosmosDbName:  'summarizer'
  }
}

resource serviceBus 'Microsoft.ServiceBus/namespaces@2021-06-01-preview' = {
  name: 'sb-${uniqueSuffix}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
}

output managedIdentityPrincipalId string = managedIdentity.properties.principalId
output keyVaultName string = keyVault.outputs.vaultName
output vaultManagedIdentityClientId string = keyVault.outputs.vaultManagedIdentityClientId
output serviceBusConnectionString string = 'Endpoint=sb://${serviceBus.name}.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=${listKeys('${serviceBus.id}/AuthorizationRules/RootManageSharedAccessKey', serviceBus.apiVersion).primaryKey}'
output cosmosDbName string = cosmos.outputs.cosmosDbName
output cosmosUrl string = cosmos.outputs.cosmosUrl
output cosmosKey string = cosmos.outputs.cosmosKey
output cosmosCollectionName string = cosmos.outputs.cosmosCollectionName
