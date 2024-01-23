using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RestaurantApi3.Authorization.requirements;

namespace RestaurantApi3.Authorization.handlers;


public class MinimumAgeHandler: AuthorizationHandler<MinimumAgeRequirement>
{
    private readonly ILogger<MinimumAgeHandler> _logger;

    public MinimumAgeHandler(ILogger<MinimumAgeHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
    {
        var userEmailClaim = context.User.FindFirst(c => c.Type == ClaimTypes.Name);
        if (userEmailClaim is null)
        {
            _logger.LogInformation("Unable to authorize user because there is no email");
            return Task.CompletedTask;
        }

        // var dateOfBirthClaim = context.User.Claims.FirstOrDefault(c => c.Type == "DateOfBirth");
        var dateOfBirthClaim = context.User.FindFirst(c => c.Type == "DateOfBirth");

        _logger.LogInformation($"Trying to authorize user: {userEmailClaim.Value}");

        if (dateOfBirthClaim is null)
        {
            _logger.LogInformation($"Authorization failed for user: {userEmailClaim.Value}, no required claim");
            return Task.CompletedTask;
        }
        
        var dateOfBirth = DateTime.Parse(dateOfBirthClaim.Value);
        // var dateOfBirth = Convert.ToDateTime(dateOfBirthClaim.Value);

        if (dateOfBirth.AddYears(requirement.MinimumAge) <= DateTime.Today)
        {
            _logger.LogInformation($"Authorization succeeded for user: {userEmailClaim.Value}");
            context.Succeed(requirement);
        }
        
        _logger.LogInformation($"Authorization failed for user: {userEmailClaim.Value}, user too young");
        return Task.CompletedTask;
    }
}