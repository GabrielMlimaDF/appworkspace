using apitesteserverlinux.Domain.Entities;

namespace apitesteserverlinux.Domain.Repositories;

public interface ITenantRepository
{
    Task AddAsync(Tenant tenant);
    Task<Tenant?> GetByIdAsync(Guid id);
}