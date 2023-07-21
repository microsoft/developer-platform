/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Reflection;

namespace Microsoft.Developer.Entities;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SoftDeleteAttribute : Attribute
{
    public static int? GetSoftDeleteTTL<T>()
        => GetSoftDeleteTTL(typeof(T));

    public static int? GetSoftDeleteTTL(Type containerType)
    {
        if (containerType is null)
            throw new ArgumentNullException(nameof(containerType));

        var attribute = containerType
            .GetCustomAttribute<SoftDeleteAttribute>();

        return attribute?.TTL;
    }

    public SoftDeleteAttribute(int ttl)
    {
        if (ttl <= 0)
            throw new ArgumentException("Value must be greater than 0", nameof(ttl));

        TTL = ttl;
    }

    public int TTL { get; }
}
