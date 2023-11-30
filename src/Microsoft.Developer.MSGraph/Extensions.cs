// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Developer.MSGraph;

internal static partial class InternalExtensions
{
    internal static bool StartsWithHttp(this string value)
        => value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
        || value.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

    internal static string? EncodeBase64(this Encoding encoding, string text)
    {
        if (text is null)
        {
            return null;
        }

        var textAsBytes = encoding.GetBytes(text);
        return Convert.ToBase64String(textAsBytes);
    }

    public static string? DecodeBase64(this Encoding encoding, string encodedText)
    {
        if (encodedText is null)
        {
            return null;
        }

        var textAsBytes = Convert.FromBase64String(encodedText);
        return encoding.GetString(textAsBytes);
    }

    internal static bool IsGuid(this string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return Guid.TryParse(value, out var _);
    }

    private static readonly Regex eMailExpression = EmailRegex();


    [GeneratedRegex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled, "en-US")]
    private static partial Regex EmailRegex();

    internal static bool IsEMail(this string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return eMailExpression.Match(value).Length > 0;
    }
}

