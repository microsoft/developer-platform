// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Entities;

public static class EntityExtensions
{
    private static string? GetLabelValue(this Metadata metadata, ProviderKey key, bool required)
        => metadata.Labels.TryGetValue(key, out var value) ? value : required ? throw new KeyNotFoundException($"Label '{key}' not found") : null;

    private static string? GetAnnotationValue(this Metadata metadata, ProviderKey key, bool required = false)
        => metadata.Labels.TryGetValue(key, out var value) ? value : required ? throw new KeyNotFoundException($"Annotation '{key}' not found") : null;

    public static string? GetLabelValue(this Entity entity, ProviderKey key) => entity.Metadata.GetLabelValue(key, required: false);

    public static string? GetLabelValue(this Metadata metadata, ProviderKey key) => metadata.GetLabelValue(key, required: false);

    public static string GetRequiredLabelValue(this Entity entity, ProviderKey key) => entity.Metadata.GetLabelValue(key, required: true)!;

    public static string GetRequiredLabelValue(this Metadata metadata, ProviderKey key) => metadata.GetLabelValue(key, required: true)!;

    public static string? GetAnnotationValue(this Entity entity, ProviderKey key) => entity.Metadata.GetAnnotationValue(key, required: false);

    public static string? GetAnnotationValue(this Metadata metadata, ProviderKey key) => metadata.GetAnnotationValue(key, required: false);

    public static string GetRequiredAnnotationValue(this Entity entity, ProviderKey key) => entity.Metadata.GetAnnotationValue(key, required: true)!;

    public static string GetRequiredAnnotationValue(this Metadata metadata, ProviderKey key) => metadata.GetAnnotationValue(key, required: true)!;
}
