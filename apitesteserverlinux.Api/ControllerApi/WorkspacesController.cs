using apitesteserverlinux.Api.Common;
using apitesteserverlinux.Api.Dtos.Workspace;
using apitesteserverlinux.Domain.Entities;
using apitesteserverlinux.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apitesteserverlinux.Api.ControllerApi;

[ApiController]
[Route("api/v1/workspaces")]
[Authorize]
public class WorkspacesController : ControllerBase
{
    private readonly IWorkspaceRepository _repository;
    private readonly IUserRepository _users;
    private readonly ITenantMemberRepository _tenantMembers;
    private readonly IWorkspaceMemberRepository _workspaceMembers;
    private readonly JwtTokenService _jwt;

    public WorkspacesController(
        IWorkspaceRepository repository,
        IUserRepository users,
        ITenantMemberRepository tenantMembers,
        IWorkspaceMemberRepository workspaceMembers,
        JwtTokenService jwt)
    {
        _repository = repository;
        _users = users;
        _tenantMembers = tenantMembers;
        _workspaceMembers = workspaceMembers;
        _jwt = jwt;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkspaceRequest model)
    {
        try
        {
            var userId = User.GetUserId();
            var tenantId = User.GetTenantId();

            if (userId is null || tenantId is null)
                return Unauthorized(ApiResponse<string>.Fail("Usuário não autenticado."));

            var user = await _users.GetByIdAsync(userId.Value);
            if (user is null)
                return Unauthorized(ApiResponse<string>.Fail("Usuário inválido."));

            var tenantMember = await _tenantMembers.GetByUserIdAsync(userId.Value);
            if (tenantMember is null || tenantMember.TenantId != tenantId.Value)
                return Forbid();

            if (tenantMember.Role != TenantRole.Admin && tenantMember.Role != TenantRole.Owner)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    ApiResponse<string>.Fail(
                        "Somente administradores podem criar workspace.",
                        new[] { "W1C1" }
                    )
                );
            }

            var workspace = new Workspace(model.Name, tenantId.Value, userId.Value);

            await _repository.AddAsync(workspace);

            var tenantMembers = await _tenantMembers.GetByTenantIdAsync(tenantId.Value);

            foreach (var member in tenantMembers)
            {
                if (member.Role != TenantRole.Admin && member.Role != TenantRole.Owner)
                    continue;

                var alreadyExists = await _workspaceMembers.ExistsAsync(workspace.Id, member.UserId);
                if (alreadyExists)
                    continue;

                var workspaceMember = new WorkspaceMember(workspace.Id, member.UserId);
                await _workspaceMembers.AddAsync(workspaceMember);
            }

            return Ok(ApiResponse<WorkspaceResponse>.Ok(new WorkspaceResponse
            {
                Id = workspace.Id,
                Name = workspace.Name,
                OwnerUserId = workspace.OwnerUserId,
                CreatedAt = workspace.CreatedAt
            }, "Workspace criado com sucesso."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<string>.Fail(
                    "Erro inesperado ao criar o workspace.",
                    new[] { "W1X1" }
                )
            );
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMyWorkspaces()
    {
        try
        {
            var userId = User.GetUserId();
            var tenantId = User.GetTenantId();

            if (userId is null || tenantId is null)
                return Unauthorized(ApiResponse<string>.Fail("Usuário não autenticado."));

            var workspaces = await _repository.GetByTenantAsync(tenantId.Value);

            var result = new List<WorkspaceResponse>();

            foreach (var workspace in workspaces)
            {
                var isMember = await _workspaceMembers.ExistsAsync(workspace.Id, userId.Value);
                if (!isMember)
                    continue;

                result.Add(new WorkspaceResponse
                {
                    Id = workspace.Id,
                    Name = workspace.Name,
                    OwnerUserId = workspace.OwnerUserId,
                    CreatedAt = workspace.CreatedAt
                });
            }

            return Ok(ApiResponse<IEnumerable<WorkspaceResponse>>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<string>.Fail(
                    "Erro inesperado ao listar workspaces.",
                    new[] { "W1X1" }
                )
            );
        }
    }

    [HttpPost("{id:guid}/select")]
    public async Task<IActionResult> Select(Guid id)
    {
        try
        {
            var userId = User.GetUserId();
            var tenantId = User.GetTenantId();

            if (userId is null || tenantId is null)
                return Unauthorized(ApiResponse<string>.Fail("Usuário não autenticado."));

            var workspace = await _repository.GetByIdAsync(id);
            if (workspace is null)
                return NotFound(ApiResponse<string>.Fail("Workspace não encontrado."));

            if (workspace.TenantId != tenantId.Value)
                return Forbid();

            var user = await _users.GetByIdAsync(userId.Value);
            if (user is null)
                return Unauthorized(ApiResponse<string>.Fail("Usuário inválido."));

            var tenantMember = await _tenantMembers.GetByUserIdAsync(user.Id);
            if (tenantMember is null || tenantMember.TenantId != tenantId.Value)
                return Forbid();

            var isWorkspaceMember = await _workspaceMembers.ExistsAsync(workspace.Id, user.Id);
            if (!isWorkspaceMember)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    ApiResponse<string>.Fail(
                        "Você não participa deste workspace.",
                        new[] { "W2A1" }
                    )
                );
            }

            var token = _jwt.CreateToken(
                userId: user.Id,
                email: user.Email,
                role: user.Role.ToString(),
                tenantId: tenantMember.TenantId,
                tenantRole: tenantMember.Role.ToString(),
                workspaceId: workspace.Id
            );

            return Ok(ApiResponse<object>.Ok(new { token }, "Workspace selecionado."));
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<string>.Fail(
                    "Erro inesperado ao selecionar workspace.",
                    new[] { "W2X1" }
                )
            );
        }
    }
}