// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;

namespace Microsoft.Developer;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class RequireLocalUserAttribute : Attribute
{
    public PathString RedirectPath { get; } = "/auth/login";

    public required string Realm { get; init; }
}
