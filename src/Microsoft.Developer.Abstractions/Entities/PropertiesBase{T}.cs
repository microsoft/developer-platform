// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities;

public abstract class PropertiesBase<T> : PropertiesBase
{
    public TNew As<TNew>()
        where TNew : PropertiesBase<T>, new()
        => this as TNew ?? new() { Properties = Properties };
}
