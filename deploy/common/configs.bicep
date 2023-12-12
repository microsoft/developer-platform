param name string

@secure()
param configs object

resource config 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: name
}

resource keyValues 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-03-01' = [for item in items(configs): {
  parent: config
  name: item.key
  properties: {
    value: item.value
  }
}]
