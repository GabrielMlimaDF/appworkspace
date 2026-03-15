namespace apitesteserverlinux.Domain.Entities;

public class Tenant
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;

    public Guid OwnerUserId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Tenant() { } // EF

    public Tenant(string name, Guid ownerUserId)
    {
        Name = name.Trim();
        OwnerUserId = ownerUserId;
    }
}