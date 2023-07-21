/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Providers;

public class MsDeveloperUserId
{
    public const string HEADER = "MS-DEVELOPER-USER-ID";

    public string UserId { get; set; }

    public string TenantId { get; set; }

    public string Id => $"{UserId}@{TenantId}";


    // userId and tenantId should be valid Guids, but that may change in the future
    public MsDeveloperUserId(string userId, string tenantId)
    {
        if (string.IsNullOrWhiteSpace(userId) || Guid.TryParse(userId, out var userIdGuid) == false || userIdGuid == Guid.Empty)
            throw new ArgumentException("Invalid MsDeveloperUserId", nameof(userId));

        if (string.IsNullOrWhiteSpace(tenantId) || Guid.TryParse(tenantId, out var tenantIdGuid) == false || tenantIdGuid == Guid.Empty)
            throw new ArgumentException("Invalid MsDeveloperUserId", nameof(tenantId));

        UserId = userId;
        TenantId = tenantId;
    }


    public MsDeveloperUserId(Guid userId, Guid tenantId)
        : this(userId.ToString(), tenantId.ToString())
    { }


    public static MsDeveloperUserId Parse(string id)
    {
        var parts = id.Split('@');

        if (parts.Length != 2)
            throw new ArgumentException("Invalid MsDeveloperUserId", nameof(id));

        return new MsDeveloperUserId(parts[0], parts[1]);
    }


    public override string ToString() => Id;


    public override bool Equals(object? obj)
    {
        if (obj is MsDeveloperUserId id)
            return id.Id == Id;

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