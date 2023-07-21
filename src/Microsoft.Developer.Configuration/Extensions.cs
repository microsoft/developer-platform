/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Developer.Configuration;

public static class Extensions
{
    public static bool TryBind<TOptions>(this IConfiguration configuration, string key, out TOptions? options)
        where TOptions : class, new()
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));

        try
        {
            options = Activator.CreateInstance<TOptions>();

            configuration.GetSection(key).Bind(options);
        }
        catch
        {
            options = null;
        }

        return options is not null;
    }
}