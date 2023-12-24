@description('Provide a location for the registry.')
param location string = resourceGroup().location

@description('Assigned Identity Id for Container Registry, as container apps needs to access images')
param containerRegistryUserAssignedIdentityId string

@description('Assigned Identity Id for Key Vault, as container apps needs to access secrets')
param keyVaultUserAssignedIdentityId string

@description('Provide a Managed Environment Id')
param managedEnvironmentId string

@description('Provide a azureContainerRegistry Name')
param azureContainerRegistryName string

param applicationInsightsConnectionString string

param external bool = false

@minLength(3)
@maxLength(32)
param appName string

@description('Container image tag, to deploy')
param image string

@description('App Settings for the container app')
param appSettings array

@description('Minimum number of replicas to run')
param minReplicas int = 0

resource containerapp 'Microsoft.App/containerApps@2022-03-01' = {
  name: appName
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${containerRegistryUserAssignedIdentityId}': {}
      '${keyVaultUserAssignedIdentityId}': {}
    }
  }
  properties: {
    managedEnvironmentId: managedEnvironmentId
    configuration: {
      ingress: {
        external: external
        targetPort: 8080
      }
      dapr: {
        enabled: true
        appId: appName
        appProtocol: 'http'
        appPort: 8080
      }
      activeRevisionsMode: 'Single'
      registries: [
        {
          server: '${azureContainerRegistryName}.azurecr.io'
          identity: containerRegistryUserAssignedIdentityId
        }
      ]
    }
    template: {
      containers: [
        {
          image: image
          name: appName
          env: union([
            {
              name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              value: applicationInsightsConnectionString
            }
          ], appSettings)
          resources: {
            cpu: json('0.75')
            memory: '1.5Gi'
          }
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: 1
      }
    }
  }
}
