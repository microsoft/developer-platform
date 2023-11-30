// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Developer;

public interface IDeveloperPlatformProviderBuilder
{
    IDeveloperPlatformBuilder Builder { get; }

    IServiceCollection Services { get; }
}
