// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using DurableTask.Core;
using DurableTask.Core.Serializing;
using Microsoft.Developer.DurableTasks;
using Microsoft.Developer.Requests;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Microsoft.Developer.Api.Providers;

internal class ProviderTemplateActivity(IProviderEntityRepository providers, IOptions<ProviderOptions> options) : AsyncTaskActivity<TemplateRequest, PostedResponse<TemplateResponse>>, IContainer<ClaimsPrincipal>, IVersionedName, IContainer<DataConverter>, IContainer<Uri>
{
    private Uri host = null!;

    private ClaimsPrincipal user = null!;

    public static string Name => nameof(ProviderTemplateActivity);

    public static string Version => "1";

    void IContainer<DataConverter>.SetItem(DataConverter converter) => DataConverter = converter;

    void IContainer<ClaimsPrincipal>.SetItem(ClaimsPrincipal item) => user = item;

    void IContainer<Uri>.SetItem(Uri item) => host = item;

    protected override Task<PostedResponse<TemplateResponse>> ExecuteAsync(TaskContext context, TemplateRequest input)
    {
        if (options.Value.TryGetValue(input.Provider, out var provider))
        {
            return providers.PostAsync(input, new DownstreamProviderOptions(user) { Providers = [provider] }, default);
        }

        return Task.FromResult(PostedResponse<TemplateResponse>.Invalid);
    }
}
