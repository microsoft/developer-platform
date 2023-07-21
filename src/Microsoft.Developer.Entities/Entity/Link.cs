/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Entities;

public class Link
{
    // A url in a standard uri format(e.g.https://example.com/some/page)
    public required string Url { get; set; }

    // A user friendly display name for the link.
    public string? Title { get; set; }

    // A key representing a visual icon to be displayed in the UI.
    public string? Icon { get; set; }

    // An optional value to categorize links into specific groups.
    public string? Type { get; set; }
}