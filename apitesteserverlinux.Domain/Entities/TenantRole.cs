namespace apitesteserverlinux.Domain.Entities;

public enum TenantRole
{
    Member = 1,
    Admin = 2,
    Owner = 3
}

public class TenantMember
{
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public TenantRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;

    private TenantMember() { } // EF

    public TenantMember(Guid tenantId, Guid userId, TenantRole role)
    {
        TenantId = tenantId;
        UserId = userId;
        Role = role;
    }
}