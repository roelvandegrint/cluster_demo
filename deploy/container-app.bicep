param containerAppName string
param location string = resourceGroup().location
param environmentId string
param containerImage string
param containerPort int
param daprAppId string
param isExternalIngress bool = false
param containerRegistry string
param containerRegistryUsername string

param environmentVars array = []

@secure()
param containerRegistryPassword string
param containerSecrets array = []

var defaultSecrets = [
  {
    name: 'registry-password'
    value: containerRegistryPassword
  }
]

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: containerAppName
  kind: 'containerapp'
  location: location
  properties: {
    kubeEnvironmentId: environmentId
    configuration: {
      secrets: concat(defaultSecrets, containerSecrets)
      registries: [
        {
          server: containerRegistry
          username: containerRegistryUsername
          passwordSecretRef: 'registry-password'
        }
      ]
      ingress: {
        external: isExternalIngress
        targetPort: containerPort
      }
    }
    template: {
      containers: [
        {
          image: containerImage
          name: containerAppName
          env: environmentVars
        }
      ]
      scale: {
        minReplicas: 1
      }
      dapr: {
        enabled: true
        appPort: containerPort
        appId: daprAppId
      }
    }
  }
}

output fqdn string = containerApp.properties.configuration.ingress.fqdn
