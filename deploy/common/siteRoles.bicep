param keyvaultName string
param configName string
param siteName string

// Role Assignments
var keyVaultSecretsOfficerRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b86a8fe4-44ce-4948-aee5-eccb2c155cd7') // Key Vault Secrets Officer
var configDataReaderRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '516239f1-63e1-4d78-a4de-a74fb236a071') // App Configuration Data Reader
// var acrPullRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d') // ACR Pull

resource site 'Microsoft.Web/sites@2022-09-01' existing = if (!empty(siteName)) {
  name: empty(siteName) ? 'siteName' : siteName
}

resource config 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: configName
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyvaultName
}

// Key Vault Secrets Officer (API)
resource keyVaultSecretOfficers 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, site.id, keyVaultSecretsOfficerRoleId)
  scope: keyVault
  properties: {
    principalId: site.identity.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: keyVaultSecretsOfficerRoleId
  }
}

// App Configuration Data Reader (Apps)
resource configDataReaders 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(config.id, site.id, configDataReaderRoleId)
  scope: config
  properties: {
    principalId: site.identity.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: configDataReaderRoleId
  }
}
