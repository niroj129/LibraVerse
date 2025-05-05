using System.Security.Claims;
using LibraVerse.Services.Interface;

namespace LibraVerse.Services;

public class UserService(IHttpContextAccessor httpContextAccessor) : IUserService
{
    public Guid? UserId
    {
        get
        {
            var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userId, out var id) ? id : null;
        }
    }
}
