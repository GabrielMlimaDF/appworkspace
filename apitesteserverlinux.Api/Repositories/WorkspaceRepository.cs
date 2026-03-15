using apitesteserverlinux.Api.Data;
using apitesteserverlinux.Domain.Entities;
using apitesteserverlinux.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace apitesteserverlinux.Api.Repositories;

public class WorkspaceRepository : IWorkspaceRepository
{
    private readonly AppDbContext _context;

    public WorkspaceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Workspace workspace)
    {
        await _context.Workspaces.AddAsync(workspace);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Workspace>> GetByTenantAsync(Guid tenantId)
    {
        return await _context.Workspaces
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Workspace?> GetByIdAsync(Guid id)
    {
        return await _context.Workspaces
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}