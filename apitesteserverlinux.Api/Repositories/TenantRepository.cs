using apitesteserverlinux.Api.Data;
using apitesteserverlinux.Domain.Entities;
using apitesteserverlinux.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace apitesteserverlinux.Api.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly AppDbContext _context;

    public TenantRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Tenant tenant)
    {
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();
    }

    public async Task<Tenant?> GetByIdAsync(Guid id)
    {
        return await _context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}