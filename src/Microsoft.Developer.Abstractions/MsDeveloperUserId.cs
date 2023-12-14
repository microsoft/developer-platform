// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Developer;

public class MsDeveloperUserId
{
    public const string HEADER = "MS-DEVELOPER-USER-ID";

    public required string UserId { get; init; }

    public required string TenantId { get; init; }

    public string Id => $"{UserId}@{TenantId}";

    public MsDeveloperUserId()
    {
    }

    public static MsDeveloperUserId Parse(string id)
    {
        if (id.Split('@') is [{ } userId, { } tenantId])
        {
            return new() { UserId = userId, TenantId = tenantId };
        }

        throw new ArgumentException("Invalid MsDeveloperUserId", nameof(id));
    }

    public override string ToString() => Id;

    public override bool Equals(object? obj)
    {
        if (obj is MsDeveloperUserId id)
        {
            return id.Id == Id;
        }

        return false;
    }

    public override int GetHashCode() => Id.GetHashCode();

    // Operators
    public static bool operator ==(MsDeveloperUserId left, MsDeveloperUserId right) => left.Equals(right);

    public static bool operator !=(MsDeveloperUserId left, MsDeveloperUserId right) => !(left == right);

    public static bool operator ==(MsDeveloperUserId left, string right) => left.Id == right;

    public static bool operator !=(MsDeveloperUserId left, string right) => !(left == right);

    public static bool operator ==(string left, MsDeveloperUserId right) => left == right.Id;

    public static bool operator !=(string left, MsDeveloperUserId right) => !(left == right);

    public static implicit operator MsDeveloperUserId(string id) => Parse(id);

    public static implicit operator string(MsDeveloperUserId id) => id.Id;
}
