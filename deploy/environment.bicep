param location string = resourceGroup().location
param environmentName string
param logAnalyticsWorkspaceName string = 'logs-${environmentName}'
param serviceBusNamespaceName string = 'sb-${environmentName}'

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-03-01-preview' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: any({
    retentionInDays: 30
    features: {
      searchVersion: 1
    }
    sku: {
      name: 'PerGB2018'
    }
  })
}

resource environment 'Microsoft.Web/kubeEnvironments@2021-02-01' = {
  name: environmentName
  location: location
  properties: {
    type: 'managed'
    internalLoadBalancerEnabled: false
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalyticsWorkspace.properties.customerId
        sharedKey: logAnalyticsWorkspace.listKeys().primarySharedKey
      }
    }
  }
}

resource serviceBus 'Microsoft.ServiceBus/namespaces@2017-04-01' = {
  name: serviceBusNamespaceName
  location: 'westeurope'

  resource newEmployeesQueue 'queues@2017-04-01' = {
    name: 'new_employees'
  }
}

output environmentId string = environment.id
