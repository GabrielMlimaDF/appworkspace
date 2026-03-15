using System.Security.Claims;

namespace apitesteserverlinux.Api.Common;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var value =
            user.FindFirstValue(ClaimTypes.NameIdentifier) ??
            user.FindFirstValue("sub");

        return Guid.TryParse(value, out var id) ? id : null;
    }

    public static Guid? GetTenantId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue("tenant_id");
        return Guid.TryParse(value, out var id) ? id : null;
    }
}