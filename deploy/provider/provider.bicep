param location string = resourceGroup().location

@description('The name of the existing api app service resource. For example myapi if the url is https://myapi.azurewebsites.net')
param api string

@description('The name of the provider app service resource. For example: myprovider will deploy an app service available at https://myprovider.azurewebsites.net')
param name string

param imageRepo string

param acrId string

param users array

@secure()
param aad object

var name_clean = replace(replace(replace(toLower(trim(name)), ' ', '-'), '_', '-'), '.', '-')
var name_clean_short = length(name_clean) <= 24 ? name_clean : take(name_clean, 24)

var name_server = name_clean_short
var name_site = name_clean_short
var name_config = name_clean_short
var name_db_account = name_clean_short
var name_keyvault = name_clean_short
var name_insights = name_clean_short
var name_storage_check = replace('${name_clean_short}store', '-', '')
var name_storage = length(name_storage_check) <= 24 ? name_storage_check : take(name_storage_check, 24)

var name_db = 'MSDevs'

var configs = {
  'AppConfig:Endpoint': config.properties.endpoint
  'AzureAd:ClientCredentials:0:ClientSecret': aad.clientSecret
  'AzureAd:ClientCredentials:0:SourceType': 'ClientSecret'
  'AzureAd:ClientId': aad.clientId
  'AzureAd:Domain': '${tenant().displayName}.onmicrosoft.com'
  'AzureAd:Instance': environment().authentication.loginEndpoint
  'AzureAd:TenantId': tenant().tenantId
}

var image = 'DOCKER|${acr.properties.loginServer}/${imageRepo}:latest'

var _acrIdParts = split(acrId, '/')
var _acr = { name: length(_acrIdParts) > 10 ? '${_acrIdParts[10]}/${_acrIdParts[8]}' : _acrIdParts[8], resourceGroup: resourceGroup(_acrIdParts[2], _acrIdParts[4]) }

resource acr 'Microsoft.ContainerRegistry/registries@2023-08-01-preview' existing = {
  name: _acr.name
  scope: _acr.resourceGroup
}

resource config 'Microsoft.AppConfiguration/configurationStores@2023-03-01' = {
  name: name_config
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    disableLocalAuth: false
    softDeleteRetentionInDays: 7
    enablePurgeProtection: false
  }
}

module config_keys '../common/configs.bicep' = {
  name: 'configs'
  params: {
    name: config.name
    configs: configs
  }
}

module database '../common/cosmos.bicep' = {
  name: 'database'
  params: {
    location: location
    accountName: name_db_account
    databaseName: name_db
    configName: config.name
  }
}

module keyvault '../common/keyvault.bicep' = {
  name: 'keyvault'
  params: {
    location: location
    name: name_keyvault
    configName: config.name
  }
}

module insights '../common/insights.bicep' = {
  name: 'insights'
  params: {
    location: location
    name: name_insights
  }
}

module userRoleAssignments '../common/userRoles.bicep' = {
  name: 'user-role-assignments'
  params: {
    users: users
    configName: config.name
    keyvaultName: keyvault.outputs.name
  }
}

resource storage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: name_storage
  location: location
  sku: {
    name: 'Standard_RAGRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: true
    allowSharedKeyAccess: true
    supportsHttpsTrafficOnly: true
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
  }
}

resource server 'Microsoft.Web/serverfarms@2022-03-01' = {
  kind: 'api,linux'
  name: name_server
  location: location
  properties: {
    reserved: true
  }
  sku: {
    name: 'P1v3'
    tier: 'PremiumV3'
  }
}

resource site 'Microsoft.Web/sites@2022-03-01' = {
  kind: 'api,linux,container'
  name: name_site
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    reserved: true
    serverFarmId: server.id
    clientAffinityEnabled: false
    siteConfig: {
      alwaysOn: true
      phpVersion: 'off'
      linuxFxVersion: image
      acrUseManagedIdentityCreds: true
      cors: {
        allowedOrigins: [
          // 'http://localhost:3000'
          'https://${name_site}.azurewebsites.net'
          'https://${api}.azurewebsites.net'
        ]
        supportCredentials: true
      }
      appSettings: [
        {
          name: 'ANCM_ADDITIONAL_ERROR_PAGE_LINK'
          value: 'https://${name_site}.scm.azurewebsites.net/detectors'
        }
        {
          name: 'AppConfig__Endpoint'
          value: config.properties.endpoint
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: insights.outputs.instrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: insights.outputs.connectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'AZURE_TENANT_ID'
          value: tenant().tenantId
        }
        {
          name: 'AZURE_TENANT_NAME'
          value: tenant().displayName
        }
        {
          name: 'AzureWebJobsDisableHomepage'
          value: 'true'
        }
        {
          name: 'AzureWebJobsFeatureFlags'
          value: 'EnableHttpProxying'
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${storage.listKeys().keys[0].value}'
        }
        {
          name: 'DOCKER_ENABLE_CI'
          value: 'true'
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_URL'
          value: 'https://${acr.properties.loginServer}'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: 'false'
        }
        {
          name: 'XDT_MicrosoftApplicationInsights_Mode'
          value: 'Recommended'
        }
      ]
    }
  }
}

module webhook '../common/webhook.bicep' = {
  name: 'webhook'
  params: {
    location: acr.location
    acrName: acr.name
    siteId: site.id
    imageRepo: imageRepo
  }
  scope: _acr.resourceGroup
}

module siteRoleAssignments '../common/siteRoles.bicep' = {
  name: 'site-role-assignments'
  params: {
    configName: config.name
    keyvaultName: keyvault.outputs.name
    siteName: site.name
  }
}
