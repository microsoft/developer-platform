// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Developer;

public static class HostingConfigurationExtensions
{
    // The Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore preview for
    // functions sets the HostingEnvironment.ApplicationName property to
    // "Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore"
    public static void SetApplicationNameFromAssembly(this IHostEnvironment env)
    {
        var entryAssemblyName = Assembly.GetEntryAssembly()?.GetName().Name
            ?? throw new InvalidOperationException("Could not get application name from entry assembly");

        env.ApplicationName = entryAssemblyName;
    }

    public static void SetApplicationNameFromAssembly(this HostBuilderContext context)
        => context.HostingEnvironment.SetApplicationNameFromAssembly();
}
