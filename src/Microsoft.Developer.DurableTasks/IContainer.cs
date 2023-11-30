// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.DurableTasks;

/// <summary>
/// An interface to allow middleware access to the task/activity until https://github.com/Azure/durabletask/pull/963 is merged
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IContainer<T>
{
    void SetItem(T item);
}
