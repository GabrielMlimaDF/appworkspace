using apitesteserverlinux.Api.Data;
using apitesteserverlinux.Domain.Entities;
using apitesteserverlinux.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace apitesteserverlinux.Api.Repositories;

public class WorkspaceMemberRepository : IWorkspaceMemberRepository
{
    private readonly AppDbContext _context;

    public WorkspaceMemberRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(WorkspaceMember workspaceMember)
    {
        await _context.WorkspaceMembers.AddAsync(workspaceMember);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid workspaceId, Guid userId)
    {
        return await _context.WorkspaceMembers
            .AsNoTracking()
            .AnyAsync(x => x.WorkspaceId == workspaceId && x.UserId == userId);
    }

    public async Task<IEnumerable<WorkspaceMember>> GetByWorkspaceAsync(Guid workspaceId)
    {
        return await _context.WorkspaceMembers
            .AsNoTracking()
            .Where(x => x.WorkspaceId == workspaceId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }
}