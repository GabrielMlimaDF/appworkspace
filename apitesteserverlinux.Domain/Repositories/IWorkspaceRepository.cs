using apitesteserverlinux.Domain.Entities;

namespace apitesteserverlinux.Domain.Repositories;

public interface IWorkspaceRepository
{
    Task AddAsync(Workspace workspace);
    Task<IEnumerable<Workspace>> GetByTenantAsync(Guid tenantId);
    Task<Workspace?> GetByIdAsync(Guid id);
}