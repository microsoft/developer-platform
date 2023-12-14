param location string = resourceGroup().location

param name string

resource acr 'Microsoft.ContainerRegistry/registries@2023-08-01-preview' = {
  name: name
  location: location
  sku: {
    name: 'Premium'
  }
  properties: {
    adminUserEnabled: true
    publicNetworkAccess: 'Enabled'
    networkRuleBypassOptions: 'AzureServices'
    dataEndpointEnabled: false
    anonymousPullEnabled: true
    networkRuleSet: {
      defaultAction: 'Allow'
    }
  }
}

output id string = acr.id
output name string = acr.name
