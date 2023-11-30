// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;

namespace Microsoft.Developer;

public static class AzureFunctionsContextExtensions
{
    public static TFeature GetRequiredFeature<TFeature>(this IInvocationFeatures features)
        => features.Get<TFeature>() ?? throw new InvalidOperationException();

    public static HttpContext GetRequiredHttpContext(this FunctionContext context)
        => context.GetHttpContext() ?? throw new InvalidOperationException($"{nameof(context)} has no http context associated with it.");
}
