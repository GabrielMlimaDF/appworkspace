namespace apitesteserverlinux.Domain.Entities;

public class Workspace
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = string.Empty;

    public Guid OwnerUserId { get; private set; }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Workspace() { } // EF Core

    public Workspace(string name, Guid tenantId, Guid ownerUserId)
    {
        Name = name.Trim();
        TenantId = tenantId;
        OwnerUserId = ownerUserId;
        CreatedAt = DateTime.UtcNow;
    }

    public void Rename(string name)
    {
        Name = name.Trim();
    }
}