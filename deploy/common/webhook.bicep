param location string = resourceGroup().location

param siteId string
param acrName string
param imageRepo string

var _siteIdParts = split(siteId, '/')
var _site = { name: length(_siteIdParts) > 10 ? '${_siteIdParts[10]}/${_siteIdParts[8]}' : _siteIdParts[8], resourceGroup: resourceGroup(_siteIdParts[2], _siteIdParts[4]) }

var name = replace('${_site.name}', '-', '')

var acrPullRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d') // ACR Pull

resource site 'Microsoft.Web/sites@2022-09-01' existing = {
  name: _site.name
  scope: _site.resourceGroup
}

resource acr 'Microsoft.ContainerRegistry/registries@2023-08-01-preview' existing = {
  name: acrName
}

resource webhook 'Microsoft.ContainerRegistry/registries/webhooks@2023-06-01-preview' = {
  name: name
  parent: acr
  location: location
  properties: {
    actions: [
      'push'
    ]
    scope: toLower(imageRepo)
    serviceUri: '${list('${site.id}/config/publishingcredentials', site.apiVersion).properties.scmUri}/api/registry/webhook'
  }
}

// ACR Pull (Apps)
resource acrPullers 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(acr.id, site.id, acrPullRoleId)
  scope: acr
  properties: {
    principalId: site.identity.principalId
    roleDefinitionId: acrPullRoleId
    principalType: 'ServicePrincipal'
  }
}
