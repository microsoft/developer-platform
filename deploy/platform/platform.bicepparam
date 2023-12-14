using 'platform.bicep'

param name = readEnvironmentVariable('DEVELOPER_PLATFORM_NAME', '<PLATFORM NAME>')

param users = [
  '00000000-0000-0000-0000-000000000000' // USER ID TO GRANT ADMIN ACCESS TO RESOURCES
]

param acrId = readEnvironmentVariable('REGISTRY_RESOURCE_ID', '/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/<REGISTRY RESOURCE GROUP>/providers/Microsoft.ContainerRegistry/registries/<REGIERY NAME>')

param imageRepo = '${readEnvironmentVariable('REGISTRY_REPOSITORY', 'developer-platform')}/api'

// replace example.secrets.yaml with secrets.yaml
param aad = loadYamlContent('example.secrets.yaml', 'aad')
