using apitesteserverlinux.Domain.Entities;

namespace apitesteserverlinux.Domain.Repositories;

public interface ITenantMemberRepository
{
    Task AddAsync(TenantMember tenantMember);
    Task<TenantMember?> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<TenantMember>> GetByTenantIdAsync(Guid tenantId);
}