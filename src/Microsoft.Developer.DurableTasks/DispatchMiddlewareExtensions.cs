// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using DurableTask.Core.Middleware;
using DurableTask.Core;

namespace Microsoft.Developer.DurableTasks;

public static class DispatchMiddlewareExtensions
{
    public static IDictionary<string, string> GetTags(this DispatchMiddlewareContext context)
        => context.GetProperty<OrchestrationExecutionContext>().OrchestrationTags;
}
