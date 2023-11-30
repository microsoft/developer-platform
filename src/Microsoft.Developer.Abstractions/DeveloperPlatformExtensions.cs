// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Extensions.DependencyInjection;

public static class DeveloperPlatformExtensions
{
    public static IDeveloperPlatformBuilder AddProvider<T>(this IDeveloperPlatformBuilder builder, Func<IDeveloperPlatformBuilder, T> factory, Action<T> configure)
        where T : IDeveloperPlatformProviderBuilder
    {
        configure(factory(builder));
        return builder;
    }

    public static IDeveloperPlatformBuilder AddDeveloperPlatform(this IServiceCollection services)
        => new Builder(services);

    private sealed record Builder(IServiceCollection Services) : IDeveloperPlatformBuilder;
}
