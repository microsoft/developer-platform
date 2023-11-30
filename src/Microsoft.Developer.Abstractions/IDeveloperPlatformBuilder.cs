// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Developer;

public interface IDeveloperPlatformBuilder
{
    IServiceCollection Services { get; }
}
