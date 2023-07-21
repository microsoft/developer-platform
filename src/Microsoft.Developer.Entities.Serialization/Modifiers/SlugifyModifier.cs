/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

using Slugify;

namespace Microsoft.Developer.Entities.Serialization;

public static class SlugifyModifier
{
    static SlugHelperConfiguration SlugConfig
    {
        get
        {
            var cfg = new SlugHelperConfiguration();
            cfg.StringReplacements.Add("@", "_");
            return cfg;
        }
    }

    public static void Modify(JsonTypeInfo jsonTypeInfo)
    {
        if (jsonTypeInfo.Kind != JsonTypeInfoKind.Object)
            return;

        foreach (var propertyInfo in jsonTypeInfo.Properties)
        {
            if (propertyInfo.PropertyType != typeof(string))
                continue;

            var attributes = propertyInfo.AttributeProvider?.GetCustomAttributes(typeof(SlugifyAttribute), true) ?? Array.Empty<object>();

            if (attributes.Length != 1)
                continue;

            var setProperty = propertyInfo.Set;

            if (setProperty is not null)
            {
                propertyInfo.Set = (obj, value) =>
                {
                    if (value != null)
                        value = new SlugHelper(SlugConfig).GenerateSlug((string)value ?? string.Empty);

                    setProperty(obj, value);
                };
            }

            // var getProperty = propertyInfo.Get;

            // if (getProperty is not null)
            //     propertyInfo.Get = (value) =>
            //     {
            //         if (value != null)
            //             value = new SlugHelper(SlugConfig).GenerateSlug((string)value ?? string.Empty);

            //         return getProperty(value!);
            //     };

        }
    }
}