using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RestaurantApi3.Authorization.requirements;
using RestaurantApi3.Entities;

namespace RestaurantApi3.Authorization.handlers;

public class ResourceOperationRequirementHandler: AuthorizationHandler<ResourceOperationRequirement, Restaurant>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement,
        Restaurant resource)
    {
        if (requirement.Operation == ResourceOperationRequirement.ResourceOperation.Read ||
            requirement.Operation == ResourceOperationRequirement.ResourceOperation.Create)
        {
            context.Succeed(requirement);
        }

        if (requirement.Operation == ResourceOperationRequirement.ResourceOperation.Delete ||
            requirement.Operation == ResourceOperationRequirement.ResourceOperation.Update)
        {
            var nameIdentifierClaim = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
            if (nameIdentifierClaim is null)
            {
                return Task.CompletedTask;
            }

            if (int.Parse(nameIdentifierClaim.Value) == resource.CreatedById)
            {
                context.Succeed(requirement);
            }
        }
        return Task.CompletedTask;
    }
}