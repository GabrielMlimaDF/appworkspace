using apitesteserverlinux.Domain.Entities;

namespace apitesteserverlinux.Domain.Repositories;

public interface IWorkspaceMemberRepository
{
    Task AddAsync(WorkspaceMember workspaceMember);
    Task<bool> ExistsAsync(Guid workspaceId, Guid userId);
    Task<IEnumerable<WorkspaceMember>> GetByWorkspaceAsync(Guid workspaceId);
}