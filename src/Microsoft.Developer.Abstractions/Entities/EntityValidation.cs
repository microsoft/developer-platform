// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;

namespace Microsoft.Developer.Entities;

public static partial class EntityValidation
{
    internal static bool IsValidPrefixAndOrSuffix(string value, char separator, Func<string, bool> isValidPrefix, Func<string, bool> isValidSuffix)
    {
        var parts = value.Split(separator);
        return parts.Length switch
        {
            1 => isValidPrefix(parts[0]),
            2 => isValidPrefix(parts[0]) && isValidSuffix(parts[1]),
            _ => false
        };
    }

    [GeneratedRegex(@"^[a-zA-Z][a-z0-9A-Z]*$", RegexOptions.None)]
    private static partial Regex KindRegex();
    public static readonly Regex KindPattern = KindRegex();


    [GeneratedRegex(@"^[a-z0-9]+(?:\-+[a-z0-9]+)*$", RegexOptions.None)]
    private static partial Regex DnsLabelRegex();
    public static readonly Regex DnsLabelPattern = DnsLabelRegex();


    [GeneratedRegex(@"^([A-Za-z0-9][-A-Za-z0-9_.]*)?[A-Za-z0-9]$", RegexOptions.None)]
    private static partial Regex ObjectNameRegex();
    public static readonly Regex ObjectNamePattern = ObjectNameRegex();


    [GeneratedRegex(@"^[a-z0-9A-Z]+$", RegexOptions.None)]
    private static partial Regex VersionNumberRegex();
    public static readonly Regex VersionNumberPattern = VersionNumberRegex();

    public static bool IsValidKind(string value)
        => !string.IsNullOrWhiteSpace(value)
        && value.Length is >= 1 and <= 63
        && KindPattern.IsMatch(value);

    // https://tools.ietf.org/html/rfc1123
    public static bool IsValidDnsSubdomain(string value)
        => !string.IsNullOrWhiteSpace(value)
        && value.Length is >= 1 and <= 253
        && value.Split('.').All(IsValidDnsLable);

    // https://tools.ietf.org/html/rfc1123
    public static bool IsValidDnsLable(string value)
        => !string.IsNullOrWhiteSpace(value)
        && value.Length is >= 1 and <= 63
        && DnsLabelPattern.IsMatch(value);

    public static bool IsValidVersionNumber(string value)
        => !string.IsNullOrWhiteSpace(value)
        && value.Length is >= 1 and <= 63
        && VersionNumberPattern.IsMatch(value);

    public static bool IsValidApiVersion(string value)
        => IsValidPrefixAndOrSuffix(value, '/', IsValidDnsSubdomain, IsValidVersionNumber);

    public static bool IsValidObjectName(string value)
        => !string.IsNullOrWhiteSpace(value)
        && value.Length is >= 1 and <= 63
        && ObjectNamePattern.IsMatch(value);

    public static bool IsValidEntityName(string value)
        => IsValidObjectName(value);

    public static bool IsValidNamespace(string value)
        => IsValidDnsLable(value);

    public static bool IsValidLabelKey(string value)
        => IsValidPrefixAndOrSuffix(value, '/', IsValidDnsSubdomain, IsValidEntityName);

    public static bool IsValidLabelValue(string value)
        => value == string.Empty || IsValidObjectName(value);

    public static bool IsValidAnnotationKey(string value)
        => IsValidPrefixAndOrSuffix(value, '/', IsValidDnsSubdomain, IsValidEntityName);
}
