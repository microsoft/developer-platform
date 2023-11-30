// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Microsoft.Developer.Entities;

public abstract class PropertiesBase
{
    protected static class Factory<TValue>
    {
        public static readonly Func<IList<TValue>> List = static () => new List<TValue>();
    }

    private IPropertyCollection? properties;

    public IPropertyCollection Properties
    {
        get => properties ??= new EntityProperties();
        init => properties = value;
    }

    protected TValue? Get<TValue>([CallerMemberName] string name = null!) => Properties.Get<TValue>(name);

    protected TValue GetOrCreate<TValue>(TValue defaultValue, [CallerMemberName] string name = null!)
        => GetOrCreateHelper<TValue>(defaultValue!, name);

    protected TValue GetOrCreate<TValue>(Func<TValue> factory, [CallerMemberName] string name = null!)
        => GetOrCreateHelper<TValue>(factory, name)!;

    private TValue GetOrCreateHelper<TValue>(object obj, string name)
    {
        ArgumentNullException.ThrowIfNull(obj);

        if (Properties.Get<TValue>(name) is { } value)
        {
            return value;
        }
        else if (obj is TValue defaultValue)
        {
            Properties.Set(defaultValue, name);
            return defaultValue;
        }
        else if (obj is Func<TValue> factory)
        {
            var newValue = factory();
            Properties.Set(newValue, name);
            return newValue;
        }

        UnknownObjectType();
        return default;

        [DoesNotReturn]
        static void UnknownObjectType() => throw new InvalidOperationException("Unknown object type");
    }

    protected void Set<TValue>(TValue? value, [CallerMemberName] string name = null!) => Properties.Set(value, name);
}
