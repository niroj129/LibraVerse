using LibraVerse.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LibraVerse.Attribute;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute(params Role[] roles) : System.Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;

        if (roles.Length != 0 && !roles.Any(r => r.ToString().Equals(roleClaim, StringComparison.OrdinalIgnoreCase)))
        {
            context.Result = new ForbidResult();
        }
    }
}