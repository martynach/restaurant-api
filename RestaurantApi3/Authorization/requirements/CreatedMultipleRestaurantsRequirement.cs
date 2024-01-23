using Microsoft.AspNetCore.Authorization;

namespace RestaurantApi3.Authorization.requirements;

public class CreatedMultipleRestaurantsRequirement: IAuthorizationRequirement
{
    public int RequiredAmount { get; }

    public CreatedMultipleRestaurantsRequirement(int requiredAmount)
    {
        RequiredAmount = requiredAmount;
    }
}