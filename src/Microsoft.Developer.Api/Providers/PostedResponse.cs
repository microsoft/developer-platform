// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer.Api.Providers;

public class PostedResponse<T>
{
    public PostedResponse()
    {
    }

    public static PostedResponse<T> Invalid { get; } = Error("Invalid request");

    public static PostedResponse<T> Error(string message) => new() { ErrorMessage = message };

    public static PostedResponse<T> Response(T result) => new() { Result = result };

    public string? ErrorMessage { get; init; }

    // Used for long running task id
    public string? Id { get; init; }

    // Finaly result
    public T? Result { get; init; }
}

