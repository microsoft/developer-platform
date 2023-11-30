// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Text;

namespace Microsoft.Developer.Hosting.Functions.Middleware;

internal struct HeaderValueBuilder(string prefix) : IEnumerable<KeyValuePair<string, string>>
{
    private Dictionary<string, string>? values;

    public void Add(string key, string value)
    {
        (values ??= [])[key] = value;
    }

    public readonly string Build()
    {
        if (values is null)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        var count = 0;

        sb.Append(prefix);
        sb.Append(' ');

        foreach (var (key, value) in values)
        {
            if (count++ != 0)
            {
                sb.Append(", ");
            }

            sb.Append(key);
            sb.Append("=\"");
            sb.Append(value);
            sb.Append('"');
        }

        return sb.ToString();
    }

    public readonly IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        => values?.GetEnumerator() ?? Enumerable.Empty<KeyValuePair<string, string>>().GetEnumerator();

    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
