using System.Security.Claims;

namespace RestaurantApi3.Services;

public class UserContextService: IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    private Claim? UserIdClaim => User?.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
    public int? UserId => UserIdClaim is null ? null : int.Parse(UserIdClaim.Value);
    
}