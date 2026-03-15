using apitesteserverlinux.Api.Common;
using apitesteserverlinux.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace apitesteserverlinux.Api.ControllerApi;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly ITenantMemberRepository _tenantMembers;
    private readonly JwtTokenService _jwt;

    public AuthController(
        IUserRepository users,
        ITenantMemberRepository tenantMembers,
        JwtTokenService jwt)
    {
        _users = users;
        _tenantMembers = tenantMembers;
        _jwt = jwt;
    }

    public record LoginRequest(string Email, string Password);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var user = await _users.GetByEmailAsync(model.Email);
        if (user is null)
            return Unauthorized(ApiResponse<string>.Fail("Credenciais inválidas."));

        var passwordHash = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(model.Password)));

        if (user.PasswordHash != passwordHash || !user.IsActive)
            return Unauthorized(ApiResponse<string>.Fail("Credenciais inválidas."));

        var tenantMember = await _tenantMembers.GetByUserIdAsync(user.Id);
        if (tenantMember is null)
            return Unauthorized(ApiResponse<string>.Fail("Usuário sem vínculo com SaaS."));

        var token = _jwt.CreateToken(
            userId: user.Id,
            email: user.Email,
            role: user.Role.ToString(),
            tenantId: tenantMember.TenantId,
            tenantRole: tenantMember.Role.ToString(),
            workspaceId: null
        );

        return Ok(ApiResponse<object>.Ok(new { token }, "Login realizado com sucesso."));
    }
}