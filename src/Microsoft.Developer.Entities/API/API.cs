/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class API : Entity<APIMetadata, APISpec, APIStatus>, IAPI<APIMetadata, APISpec, APIStatus>
{
    public override string Kind => nameof(API);
}
