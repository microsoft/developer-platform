param keyvaultName string
param configName string

param users array

// Role Assignments
var keyVaultAdminRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '00482a5a-887f-4fb3-b363-3b7fe8e74483') // Key Vault Administrator
// var keyVaultSecretsOfficerRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b86a8fe4-44ce-4948-aee5-eccb2c155cd7') // Key Vault Secrets Officer
var configDataOwnerRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '5ae67dd6-50cb-40e7-96ff-dc2bfa4b606b') // App Configuration Data Owner
// var configDataReaderRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '516239f1-63e1-4d78-a4de-a74fb236a071') // App Configuration Data Reader
// var acrPullRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d') // ACR Pull

resource config 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: configName
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyvaultName
}

// Key Vault Administrators (Users)
resource keyVaulAdmins 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for user in users: {
  name: guid(keyVault.id, user, keyVaultAdminRoleId)
  scope: keyVault
  properties: {
    principalId: user
    principalType: 'User'
    roleDefinitionId: keyVaultAdminRoleId
  }
}]

// App Configuration Data Owner (Users)
resource configDataOwners 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for user in users: {
  name: guid(config.id, user, configDataOwnerRoleId)
  scope: config
  properties: {
    principalId: user
    principalType: 'User'
    roleDefinitionId: configDataOwnerRoleId
  }
}]
