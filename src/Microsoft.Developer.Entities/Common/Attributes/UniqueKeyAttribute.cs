/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Reflection;

namespace Microsoft.Developer.Entities;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class UniqueKeyAttribute : Attribute
{
    public static IEnumerable<string> GetPaths<T>(bool camelCase = true)
        => GetPaths(typeof(T), camelCase);

    public static IEnumerable<string> GetPaths(Type type, bool camelCase = true)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        return type.GetProperties()
            .Where(p => p.GetCustomAttribute<UniqueKeyAttribute>() is not null)
            .Select(p => $"/{(camelCase ? JsonNamingPolicy.CamelCase.ConvertName(p.Name) : p.Name)}");
    }
}