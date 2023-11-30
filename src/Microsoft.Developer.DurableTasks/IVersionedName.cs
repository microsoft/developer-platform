// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Extensions.DependencyInjection;

public interface IVersionedName
{
    static abstract string Name { get; }

    static abstract string Version { get; }
}
