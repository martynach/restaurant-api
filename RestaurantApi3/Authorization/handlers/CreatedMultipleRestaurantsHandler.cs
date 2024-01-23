using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RestaurantApi3.Authorization.requirements;
using RestaurantApi3.Entities;

namespace RestaurantApi3.Authorization.handlers;

public class CreatedMultipleRestaurantsHandler: AuthorizationHandler<CreatedMultipleRestaurantsRequirement>
{
    private readonly RestaurantDbContext _dbContext;

    public CreatedMultipleRestaurantsHandler(RestaurantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreatedMultipleRestaurantsRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            return Task.CompletedTask;
        }

        var userId = int.Parse(userIdClaim.Value);

        var restaurantsCreatedByUser = _dbContext.Restaurants.Count(r => r.CreatedById == userId);
        
        if (requirement.RequiredAmount <= restaurantsCreatedByUser)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}