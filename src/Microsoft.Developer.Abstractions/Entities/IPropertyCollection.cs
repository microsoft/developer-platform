// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;

namespace Microsoft.Developer.Entities;

public interface IPropertyCollection : IEnumerable<KeyValuePair<string, object>>
{
    object? this[string key] { get; set; }

    T? Get<T>([CallerMemberName] string name = null!);

    void Set<T>(T? value, [CallerMemberName] string name = null!);
}
