// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities;

public class EntityProperties : Dictionary<string, object>, IPropertyCollection
{
    public EntityProperties()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    object? IPropertyCollection.this[string key]
    {
        get => Get<object>(key);
        set => Set(value, key);
    }

    protected virtual T? Convert<T>(string key, object obj)
        => obj is T t ? t : default;

    public T? Get<T>(string name)
    {
        if (TryGetValue(name, out var result))
        {
            return Convert<T>(name, result);
        }

        return default;
    }

    public void Set<T>(T? value, string name)
    {
        if (value is null)
        {
            Remove(name);
        }
        else
        {
            this[name] = value;
        }
    }
}
