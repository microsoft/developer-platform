/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class Project : Entity<ProjectMetadata, ProjectSpec, ProjectStatus>, IProject<ProjectMetadata, ProjectSpec, ProjectStatus>
{
    public override string Kind => nameof(Project);
}
