// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Providers.JsonSchema;

public static class JsonSchemaTypes
{
    public const string Array = "array";
    public const string Boolean = "boolean";
    public const string Integer = "integer";
    public const string Number = "number";
    public const string Object = "object";
    public const string String = "string";
    public static readonly string[] All = [Array, Boolean, Integer, Number, Object, String];
}
