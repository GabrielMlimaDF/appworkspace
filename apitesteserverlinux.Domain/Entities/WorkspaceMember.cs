namespace apitesteserverlinux.Domain.Entities;

public class WorkspaceMember
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid WorkspaceId { get; private set; }

    public Guid UserId { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private WorkspaceMember() { } // EF Core

    public WorkspaceMember(Guid workspaceId, Guid userId)
    {
        WorkspaceId = workspaceId;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }
}