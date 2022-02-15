param namespaceName string = 'sb-cluster_demo'
param location string = resourceGroup().location

resource serviceBus 'Microsoft.ServiceBus/namespaces@2017-04-01' = {
  name: namespaceName
  location: location

  resource newEmployeesTopic 'topics@2017-04-01' = {
    name: 'new_employees'
  }
}
