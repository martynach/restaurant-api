using Microsoft.AspNetCore.Authorization;

namespace RestaurantApi3.Authorization.requirements;

public class MinimumAgeRequirement: IAuthorizationRequirement
{
    public int MinimumAge { get; }

    public MinimumAgeRequirement(int minimumAge)
    {
        MinimumAge = minimumAge;
    }
}