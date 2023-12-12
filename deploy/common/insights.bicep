param location string = resourceGroup().location

param name string = resourceGroup().name

// param configName string = ''

// var configs = {
//   APPINSIGHTS_INSTRUMENTATIONKEY: insights.properties.InstrumentationKey
//   APPLICATIONINSIGHTS_CONNECTION_STRING: insights.properties.ConnectionString
// }

resource workspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: name
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
    workspaceCapping: {
      dailyQuotaGb: -1
    }
    publicNetworkAccessForQuery: 'Enabled'
    publicNetworkAccessForIngestion: 'Enabled'
  }
}

resource insights 'Microsoft.Insights/components@2020-02-02' = {
  name: name
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: workspace.id
    RetentionInDays: 90
    IngestionMode: 'LogAnalytics'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

// module insights_configs 'configs.bicep' = if (!empty(configName)) {
//   name: 'insights-configs'
//   params: {
//     name: configName
//     configs: configs
//   }
// }

output instrumentationKey string = insights.properties.InstrumentationKey
output connectionString string = insights.properties.ConnectionString
