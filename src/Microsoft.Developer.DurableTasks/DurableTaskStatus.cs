// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using DurableTask.Core;

namespace Microsoft.Developer.DurableTasks;

public sealed class DurableTaskStatus(OrchestrationState state)
{
    public OrchestrationStatus State => state.OrchestrationStatus;

    public string? FailureMessage => state.FailureDetails?.ErrorMessage;

    public DateTimeOffset Created => state.CreatedTime;

    public DateTimeOffset Completed => state.CompletedTime;

    public DateTimeOffset LastModified => state.LastUpdatedTime;

    public string? Parent => state.ParentInstance?.OrchestrationInstance?.InstanceId;
}
