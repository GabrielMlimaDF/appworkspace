using apitesteserverlinux.Api.Data;
using apitesteserverlinux.Domain.Entities;
using apitesteserverlinux.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace apitesteserverlinux.Api.Repositories;

public class TenantMemberRepository : ITenantMemberRepository
{
    private readonly AppDbContext _context;

    public TenantMemberRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TenantMember tenantMember)
    {
        await _context.TenantMembers.AddAsync(tenantMember);
        await _context.SaveChangesAsync();
    }

    public async Task<TenantMember?> GetByUserIdAsync(Guid userId)
    {
        return await _context.TenantMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }
}