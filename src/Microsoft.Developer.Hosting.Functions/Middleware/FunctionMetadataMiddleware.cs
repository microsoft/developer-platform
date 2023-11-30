// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Developer.Features;
using System.Collections.Concurrent;
using System.Reflection;

namespace Microsoft.Developer.Hosting.Functions.Middleware;

/// <summary>
/// Populates the <see cref="FunctionContext.Features"/> with an entry for <see cref="IFunctionMetadataFeature"/> to access
/// metadata for the function.
/// </summary>
internal class FunctionMetadataMiddleware : IFunctionsWorkerMiddleware
{
    public ConcurrentDictionary<string, IFunctionMetadataFeature> _metadataCache = new();

    private readonly Feature empty = new([]);

    public Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var metadata = _metadataCache.GetOrAdd(context.FunctionId, CreateMetadata, context.FunctionDefinition);

        context.Features.Set(metadata);

        return next(context);
    }

    private Feature CreateMetadata(string functionId, FunctionDefinition definition)
    {
        if (GetTargetFunctionMethod(definition) is { } m)
        {
            var fromMethod = m.GetCustomAttributes(inherit: true);
            var fromClass = m?.DeclaringType?.GetCustomAttributes(inherit: true);

            return (fromMethod, fromClass) switch
            {
                (null, null) => empty,
                (_, null) => new(fromMethod),
                (null, _) => new(fromClass),
                _ => new([.. fromMethod, .. fromClass]),
            };
        }

        return empty;
    }

    private class Feature(object[] metadata) : IFunctionMetadataFeature
    {
        public EndpointMetadataCollection Metadata { get; } = new(metadata);
    }

    // No in-box support to get the current function MethodInfo to retrieve attributes :(
    // Tracking issue: https://github.com/Azure/azure-functions-dotnet-worker/issues/903
    public static MethodInfo? GetTargetFunctionMethod(FunctionDefinition definition)
    {
        var entryPoint = definition.EntryPoint;

        var idx = entryPoint.LastIndexOf('.');

        if (idx == -1)
        {
            return null;
        }

        var typeName = entryPoint[..idx];
        var methodName = entryPoint[(idx + 1)..];

        return Assembly.LoadFrom(definition.PathToAssembly)
            .GetType(typeName)
            ?.GetMethod(methodName);
    }
}
