/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public interface IStatus : IAdditionalProperties
{
    List<IStatusItem>? Items { get; set; }
}

public interface IStatus<TItem> : IStatus
    where TItem : class, IStatusItem
{
    new List<TItem>? Items { get; set; }
}


public interface IStatusItem : IAdditionalProperties
{
    string Type { get; set; }
    StatusItemLevel? Level { get; set; }
    string? Message { get; set; }
    IStatusItemError? Error { get; set; }
}

public interface IStatusItemError
{
    string? Name { get; set; }
    string? Message { get; set; }
    string? Code { get; set; }
}

public enum StatusItemLevel
{
    Info,
    Warning,
    Error
}