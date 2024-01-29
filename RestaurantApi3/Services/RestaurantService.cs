using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RestaurantApi3.Authorization.requirements;
using RestaurantApi3.Dtos;
using RestaurantApi3.Entities;
using RestaurantApi3.Exceptions;

namespace RestaurantApi3.Services;

public class RestaurantService : IRestaurantService
{
    private ILogger<RestaurantService> _logger;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;
    private RestaurantDbContext _context;

    public RestaurantService(IMapper mapper,
        RestaurantDbContext context,
        ILogger<RestaurantService> logger,
        IAuthorizationService authorizationService,
        IUserContextService userContextService)
    {
        _mapper = mapper;
        _context = context;
        _logger = logger;
        _authorizationService = authorizationService;
        _userContextService = userContextService;
    }

    public PagedResult<RestaurantDto> GetAll(RestaurantQuery query)
    {

        var baseQuery = _context.Restaurants
            .Where(r => query.SearchBy == null || r.Name.ToLower().Contains(query.SearchBy.ToLower()) ||
                        r.Description.ToLower().Contains(query.SearchBy.ToLower()));

        if (!String.IsNullOrEmpty(query.SortBy))
        {
            var sortColumnSelector = new Dictionary<string, Expression<Func<Restaurant, String>>>()
            {
                { nameof(Restaurant.Name), r => r.Name },
                { nameof(Restaurant.Description), r => r.Description },
                { nameof(Restaurant.Category), r => r.Category },
            };

            baseQuery = query.SortOrder == SortOrder.ASC
                ? baseQuery.OrderBy(sortColumnSelector[query.SortBy])
                : baseQuery.OrderByDescending(sortColumnSelector[query.SortBy]);
        }
        
        baseQuery = baseQuery
            .Include(r => r.Dishes)
            .Include(r => r.Address);
        
        var restaurants = baseQuery
            .Skip(query.PageSize * (query.PageNumber - 1))
            .Take(query.PageSize)
            .ToList();
        
        int totalRestaurantsCount = baseQuery.Count();


        var dtos = _mapper.Map<List<RestaurantDto>>(restaurants);


        var pagedResult = new PagedResult<RestaurantDto>(dtos, totalRestaurantsCount, query.PageSize, query.PageNumber);

        return pagedResult;
    }

    public RestaurantDto GetById(int id)
    {
        var restaurant = _context.Restaurants
            .Include(r => r.Dishes)
            .Include(r => r.Address)
            .FirstOrDefault(r => r.Id == id);

        if (restaurant is null)
        {
            throw new NotFoundException($"Restaurant with id: {id} not found.");
        }

        return _mapper.Map<RestaurantDto>(restaurant);
    }

    public int CreateRestaurant(CreateRestaurantDto dto)
    {
        var restaurant = _mapper.Map<Restaurant>(dto);

        var userId = _userContextService.UserId;
        if (userId is null)
        {
            throw new ForbidException("User id required");
        }

        restaurant.CreatedById = userId;
        _context.Restaurants.Add(restaurant);
        _context.SaveChanges();
        return restaurant.Id;
    }

    public void DeleteRestaurant(int id)
    {
        _logger.LogWarning(AppConstants.LoggerPrefix + $"Restaurant with id {id} DELETE action invoked.");

        var restaurant = _context.Restaurants.FirstOrDefault(r => r.Id == id);

        if (restaurant is null)
        {
            throw new NotFoundException($"Restaurant with id: {id} not found.");
        }

        ClaimsPrincipal? user = _userContextService.User;
        if (user is null)
        {
            throw new ForbidException("User is required (Authorization header)");
        }

        var requirement = new ResourceOperationRequirement(ResourceOperationRequirement.ResourceOperation.Update);
        var authorizationResult = _authorizationService.AuthorizeAsync(user, restaurant, requirement).Result;

        if (!authorizationResult.Succeeded)
        {
            throw new ForbidException($"User cannot delete restaurant created by someone else");
        }

        _context.Restaurants.Remove(restaurant);
        _context.SaveChanges();
    }

    public void UpdateRestaurant(int restaurantId, UpdateRestaurantDto updateDto)
    {
        var restaurant = _context.Restaurants.FirstOrDefault(r => r.Id == restaurantId);
        if (restaurant is null)
        {
            throw new NotFoundException($"Restaurant with id: {restaurantId} not found.");
        }

        ClaimsPrincipal? user = _userContextService.User;
        if (user is null)
        {
            throw new ForbidException("User is required (Authorization header)");
        }

        var requirement = new ResourceOperationRequirement(ResourceOperationRequirement.ResourceOperation.Update);
        var authorizationResult = _authorizationService.AuthorizeAsync(user, restaurant, requirement).Result;

        if (!authorizationResult.Succeeded)
        {
            throw new ForbidException($"User cannot update restaurant created by someone else");
        }

        restaurant.Description = updateDto.Description ?? restaurant.Description;
        restaurant.Name = updateDto.Name ?? restaurant.Name;
        restaurant.HasDelivery = updateDto.HasDelivery ?? restaurant.HasDelivery;

        _context.SaveChanges();
        
        
        
        
        
    }
}