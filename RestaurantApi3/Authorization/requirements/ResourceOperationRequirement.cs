using Microsoft.AspNetCore.Authorization;

namespace RestaurantApi3.Authorization.requirements;

public class ResourceOperationRequirement: IAuthorizationRequirement
{

    public enum ResourceOperation
    {
        Create,
        Read,
        Update,
        Delete
    }
    
    public ResourceOperation Operation { get; }



    public ResourceOperationRequirement(ResourceOperation resourceOperation)
    {
        Operation = resourceOperation;
    }
}