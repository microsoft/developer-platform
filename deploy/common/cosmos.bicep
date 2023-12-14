param location string = resourceGroup().location

param accountName string
param databaseName string = 'MSDevs'

param configName string = ''

var configs = {
  'Cosmos:Endpoint': database.properties.documentEndpoint
  'Cosmos:DatabaseName': databaseName
  'Cosmos:ConnectionString': database.listConnectionStrings().connectionStrings[0].connectionString
}

resource database 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: accountName
  location: location
  kind: 'GlobalDocumentDB'
  identity: {
    type: 'None'
  }
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
    capacity: {
      totalThroughputLimit: 4000
    }
  }

  resource db 'sqlDatabases' = {
    name: databaseName
    properties: {
      resource: {
        id: databaseName
      }
    }
  }
}

module database_configs 'configs.bicep' = if (!empty(configName)) {
  name: '${accountName}-db-configs'
  params: {
    name: configName
    configs: configs
  }
}
