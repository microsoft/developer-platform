param location string = resourceGroup().location

param name string

param configName string

var configs = {
  'KeyVault:VaultUri': keyvault.properties.vaultUri
}

resource keyvault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: name
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enableRbacAuthorization: true
    publicNetworkAccess: 'Enabled'
    enablePurgeProtection: true
  }
}

module keyvault_configs 'configs.bicep' = if (!empty(configName)) {
  name: '${name}-kv-configs'
  params: {
    name: configName
    configs: configs
  }
}

output name string = keyvault.name
