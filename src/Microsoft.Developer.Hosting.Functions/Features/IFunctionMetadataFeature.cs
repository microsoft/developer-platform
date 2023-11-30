using Microsoft.AspNetCore.Http;

namespace Microsoft.Developer.Features;

/// <summary>
/// A feature to access the metadata of the current feature. This currently includes:
/// 
/// <list type="bullet">
/// <item>Attributes on the method</item>
/// <item>Attributes on the containing type</item>
/// </list>
/// </summary>
/// <remarks>This is a feature gap in the product tracked here: https://github.com/Azure/azure-functions-dotnet-worker/issues/903</remarks>
public interface IFunctionMetadataFeature
{
    public EndpointMetadataCollection Metadata { get; }
}
