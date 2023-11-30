// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using DurableTask.Core;
using DurableTask.Core.Serializing;
using Microsoft.Developer.DurableTasks;
using Microsoft.Developer.Requests;

namespace Microsoft.Developer.Api.Providers;

internal class TemplateOrchestration : TaskOrchestration<TemplateResponse, TemplateRequest>, IVersionedName, IContainer<DataConverter>
{
    public static string Name => nameof(TemplateOrchestration);

    public static string Version => "1";

    void IContainer<DataConverter>.SetItem(DataConverter converter) => DataConverter = converter;

    public override async Task<TemplateResponse> RunTask(OrchestrationContext context, TemplateRequest input)
    {
        context.MessageDataConverter = (JsonDataConverter)DataConverter;
        var response = await context.ScheduleAsyncTask<ProviderTemplateActivity, TemplateRequest, PostedResponse<TemplateResponse>>(input);

        while (true)
        {
            if (response is { Result: { } finalResult })
            {
                return finalResult;
            }
            else if (response is { Id: { Length: > 0 } id })
            {
                response = await context.ScheduleAsyncTask<ProviderCheckStatusActivity, ProviderStatusCheck, PostedResponse<TemplateResponse>>(new ProviderStatusCheck { Id = id, Provider = input.Provider });
            }
            else
            {
                // TODO handle other errors
                return new();
            }
        }
    }
}
