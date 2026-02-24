using apitesteserverlinux.Api.Data;
using apitesteserverlinux.Domain.Entities;
using apitesteserverlinux.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

public class WorkspaceRepository : IWorkspaceRepository
{
    private readonly AppDbContext _context;

    public WorkspaceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Workspace workspace)
    {
        _context.Workspaces.Add(workspace);
        await _context.SaveChangesAsync();
    }

    public Task<Workspace?> GetByIdAsync(Guid id)
        => _context.Workspaces.FirstOrDefaultAsync(w => w.Id == id);

    public async Task<IEnumerable<Workspace>> GetByOwnerAsync(Guid ownerUserId)
        => await _context.Workspaces
            .Where(w => w.OwnerUserId == ownerUserId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
}
