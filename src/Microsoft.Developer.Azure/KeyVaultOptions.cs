// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Configuration.Options;

public class KeyVaultOptions
{
    public const string Section = "KeyVault";

    public string Endpoint { get; set; } = string.Empty;
}
