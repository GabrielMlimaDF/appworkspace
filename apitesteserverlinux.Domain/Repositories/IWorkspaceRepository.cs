using apitesteserverlinux.Domain.Entities;

namespace apitesteserverlinux.Domain.Repositories;
public interface IWorkspaceRepository
{
    Task AddAsync(Workspace workspace);
    Task<Workspace?> GetByIdAsync(Guid id);
    Task<IEnumerable<Workspace>> GetByOwnerAsync(Guid ownerUserId);
}
