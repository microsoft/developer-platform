/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Reflection;

using System.Collections.Concurrent;

namespace Microsoft.Developer.Entities;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class PartitionKeyAttribute : Attribute
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo> PropertyCache = new();

    private static PropertyInfo GetProperty(Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        return PropertyCache.GetOrAdd(type, (key) =>
        {
            var property = type.GetProperties()
                .Where(p => p.GetCustomAttribute<PartitionKeyAttribute>(true) is not null)
                .SingleOrDefault() ?? throw new NotSupportedException($"Did not find {typeof(PartitionKeyAttribute)} on type {type}");

            if (property.PropertyType != typeof(string))
                throw new NotSupportedException($"{typeof(PartitionKeyAttribute)} is only supported on properties with the type String");

            return property;
        });
    }

    public static string? GetValue<T>(T obj)
        where T : class
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        return GetProperty(typeof(T)).GetValue(obj) as string;
    }

    public static string? GetPath<T>(bool camelCase = true)
        => GetPath(typeof(T), camelCase);

    public static string? GetPath(Type type, bool camelCase = true)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        var name = GetProperty(type)?.Name;

        if (string.IsNullOrEmpty(name))
            return name;

        if (camelCase)
            name = JsonNamingPolicy.CamelCase.ConvertName(name);

        return $"/{name}";
    }
}
