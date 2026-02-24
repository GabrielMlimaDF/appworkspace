using apitesteserverlinux.Api.Common;
using apitesteserverlinux.Api.Dtos.Workspace;
using apitesteserverlinux.Domain.Entities;
using apitesteserverlinux.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/v1/workspaces")]
public class WorkspacesController : ControllerBase
{
    private readonly IWorkspaceRepository _repository;

    public WorkspacesController(IWorkspaceRepository repository)
    {
        _repository = repository;
    }

    private bool TryGetUserId(out Guid userId)
    {
        userId = Guid.Empty;

        var value =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub");

        return Guid.TryParse(value, out userId);
    }

    // POST api/v1/workspaces
    // Futuro: [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkspaceRequest model)
    {
        try
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<string>.Fail("Usuário não autenticado."));

            var workspace = new Workspace(model.Name, userId);

            await _repository.AddAsync(workspace);

            return Ok(ApiResponse<WorkspaceResponse>.Ok(new WorkspaceResponse
            {
                Id = workspace.Id,
                Name = workspace.Name,
                OwnerUserId = workspace.OwnerUserId,
                CreatedAt = workspace.CreatedAt
            }, "Workspace criado com sucesso."));
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<string>.Fail("Erro inesperado ao criar o workspace.", new[] { "W1X1" })
            );
        }
    }

    // GET api/v1/workspaces
    // Futuro: [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetMyWorkspaces()
    {
        try
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<string>.Fail("Usuário não autenticado."));

            var workspaces = await _repository.GetByOwnerAsync(userId);

            var result = workspaces.Select(w => new WorkspaceResponse
            {
                Id = w.Id,
                Name = w.Name,
                OwnerUserId = w.OwnerUserId,
                CreatedAt = w.CreatedAt
            });

            return Ok(ApiResponse<IEnumerable<WorkspaceResponse>>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<string>.Fail("Erro inesperado ao listar workspaces.", new[] { "W1X1" })
            );
        }
    }
}
