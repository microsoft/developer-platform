// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class DisplayAsStringAttribute : Attribute
{
    private int? minLength;
    private int? maxLength;

    public bool OnlyString { get; set; }

    public string? Description { get; set; }

    public string? RegularExpression { get; set; }

    public string? DefaultValue { get; set; }

    public int MinLength
    {
        get => minLength.GetValueOrDefault();
        set => minLength = value;
    }

    public int? GetMinLength() => minLength;

    public int? GetMaxLength() => maxLength;

    public int MaxLength
    {
        get => maxLength.GetValueOrDefault();
        set => maxLength = value;
    }
}