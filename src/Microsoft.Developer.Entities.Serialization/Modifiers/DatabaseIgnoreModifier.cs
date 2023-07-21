/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities.Serialization;

public static class DatabaseIgnoreModifier
{
    public static void Modify(JsonTypeInfo jsonTypeInfo)
    {
        if (jsonTypeInfo.Kind != JsonTypeInfoKind.Object)
            return;

        jsonTypeInfo.Properties
            .RemoveAll(p => p.AttributeProvider?
                .GetCustomAttributes(typeof(DatabaseIgnoreAttribute), true).Length == 1);
    }
}