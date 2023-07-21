/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public abstract class Status<TItem> : IStatus<TItem>
    where TItem : class, IStatusItem
{
    public List<TItem>? Items
    {
        get => ((IStatus)this).Items as List<TItem> ?? default!;
        set => ((IStatus)this).Items = value?.Cast<IStatusItem>().ToList();
    }

    List<IStatusItem>? IStatus.Items { get; set; }

    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public abstract class Status : Status<StatusItem>
{
}

public class StatusItem : IStatusItem
{
    public string Type { get; set; } = default!;
    public StatusItemLevel? Level { get; set; }
    public string? Message { get; set; }
    public IStatusItemError? Error { get; set; }
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public class StatusItemError : IStatusItemError
{
    public string? Name { get; set; }
    public string? Message { get; set; }
    public string? Code { get; set; }
}