/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using System.Reflection;

namespace Microsoft.Developer.Entities;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public sealed class ContainerNameAttribute : Attribute
{
    public static string GetNameOrDefault<T>()
        => GetNameOrDefault(typeof(T));

    public static string GetNameOrDefault(Type containerType)
    {
        if (containerType is null)
            throw new ArgumentNullException(nameof(containerType));

        var attribute = containerType
            .GetCustomAttribute<ContainerNameAttribute>();

        return attribute?.Name ?? containerType.Name;
    }

    public ContainerNameAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value must not NULL, EMPTY, or WHITESPACE", nameof(name));

        Name = name.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    public string Name { get; }
}
