using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace apitesteserverlinux.Api.Common;

public class JwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public string CreateToken(
     Guid userId,
     string email,
     string role,
     Guid tenantId,
     string tenantRole,
     Guid? workspaceId = null)
    {
        var claims = new List<Claim>
    {
        new("sub", userId.ToString()),
        new("email", email),
        new("role", role),
        new("tenant_id", tenantId.ToString()),
        new("tenant_role", tenantRole),
    };

        if (workspaceId is not null)
            claims.Add(new Claim("workspace_id", workspaceId.Value.ToString()));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpireMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public string CreateWorkspaceToken(Guid userId, string email, string role, Guid workspaceId)
    {
        var claims = new List<System.Security.Claims.Claim>
    {
        new("sub", userId.ToString()),
        new("email", email),
        new("role", role),
        new("workspace_id", workspaceId.ToString())
    };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.Secret));

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpireMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
