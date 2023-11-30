// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Extensions.Hosting;

public static class HostEnvironmentExtensions
{
    public static readonly string DesignTime = nameof(DesignTime);

    public static bool IsDesignTime(this IHostEnvironment hostEnvironment)
    {
        ArgumentNullException.ThrowIfNull(hostEnvironment);

        return hostEnvironment.IsEnvironment(DesignTime);
    }
}