// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property)]
public sealed class ExampleAttribute(string example) : Attribute
{
    public string Example => example;
}
