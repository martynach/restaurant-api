using System.Security.Claims;

namespace RestaurantApi3.Services;

public interface IUserContextService
{

    public ClaimsPrincipal? User { get; }

    public int? UserId { get; }

}