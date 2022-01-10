param location string = resourceGroup().location
param environmentName string
param frontendImage string
param frontendPort int
param frontendAppId string
param staffingsvcImage string
param staffingsvcPort int
param staffingsvcAppId string
param registry string
param registryUsername string
@secure()
param registryPassword string

// Container Apps Environment (environment.bicep)
module environment 'environment.bicep' = {
  name: 'container-app-environment'
  params: {
    environmentName: environmentName
    location: location
  }
}

// Container-2-Dotnet (container-app.bicep)
// We deploy it first so we can call it from the node-app
module staffingsvc 'container-app.bicep' = {
  name: 'staffing-svc'
  params: {
    containerAppName: 'cont-staffingsvc'
    location: location
    environmentId: environment.outputs.environmentId
    containerImage: staffingsvcImage
    containerPort: staffingsvcPort
    daprAppId: staffingsvcAppId
    containerRegistry: registry
    containerRegistryUsername: registryUsername
    containerRegistryPassword: registryPassword
    isExternalIngress: true
    containerSecrets: []
    environmentVars: []
  }
}

// Container-1-Node (container-app.bicep)
module frontend 'container-app.bicep' = {
  name: 'frontend'
  params: {
    containerAppName: 'cont-frontend'
    location: location
    environmentId: environment.outputs.environmentId
    containerImage: frontendImage
    containerPort: frontendPort
    daprAppId: frontendAppId
    containerRegistry: registry
    containerRegistryUsername: registryUsername
    containerRegistryPassword: registryPassword
    isExternalIngress: true
    environmentVars: [
      {
        name: 'ASPNETCORE_ENVIRONMENT'
        value: 'Development'
      }
    ]
  }
}

// Container-1-Node (container-app.bicep)
// module mvcDefault 'container-app.bicep' = {
//   name: 'blazor-server'
//   params: {
//     containerAppName: 'blazor-server'
//     location: location
//     environmentId: environment.outputs.environmentId
//     containerImage: mvcImage
//     containerPort: mvcPort
//     containerRegistry: registry
//     containerRegistryUsername: registryUsername
//     containerRegistryPassword: registryPassword
//     isExternalIngress: true
//     environmentVars: []
//   }
// }
