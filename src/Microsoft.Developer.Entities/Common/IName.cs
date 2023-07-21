/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Slugify;

namespace Microsoft.Developer.Entities;

public interface IName
{
    static SlugHelperConfiguration config
    {
        get
        {
            var cfg = new SlugHelperConfiguration();
            cfg.StringReplacements.Add("@", "_");
            return cfg;
        }
    }

    public static string CreateSlug(string instance)
        => instance is null
        ? throw new ArgumentNullException(nameof(instance))
        : new SlugHelper(config).GenerateSlug(instance ?? string.Empty);

    public string Name { get; set; }
}