/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Configuration.Options;

public class AppConfigOptions
{
    public const string Section = "AppConfig";

    public string Endpoint { get; set; } = string.Empty;
}