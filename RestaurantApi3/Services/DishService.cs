using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantApi3.Dtos;
using RestaurantApi3.Entities;
using RestaurantApi3.Exceptions;

namespace RestaurantApi3.Services;

public class DishService: IDishService
{
    private readonly ILogger<DishService> _logger;
    private readonly RestaurantDbContext _context;
    private readonly IMapper _mapper;

    public DishService(ILogger<DishService> logger, RestaurantDbContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }
    
    public IEnumerable<DishDto> GetAll(int restaurantId)
    {
        // // my solution
        // var dishes = _context.Dishes
        //     .Where(d => d.RestaurantId == restaurantId).ToList();
        //
        // var dishDtos = _mapper.Map<List<DishDto>>(dishes);

        var restaurant = GetRestaurantById(restaurantId);
        var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes);
        return dishDtos;
    }

    public DishDto GetDishById(int restaurantId, int dishId)
    {
        var dish = _context.Dishes
            .FirstOrDefault(d => d.RestaurantId == restaurantId && d.Id == dishId);

        if (dish is null)
        {
            throw new NotFoundException($"Dish with id: {dishId} for restaurant with id: {restaurantId} not found.");
        }

        var dishDto = _mapper.Map<DishDto>(dish);
        return dishDto;
        
    }

    public int CreateDish(int restaurantId, CreateDishDto dishDto)
    {
        var restaurant = GetRestaurantById(restaurantId);
        var dish = _mapper.Map<Dish>(dishDto);
        dish.RestaurantId = restaurantId;
        
        _context.Dishes.Add(dish);
        _context.SaveChanges();

        return dish.Id;
    }

    public void DeleteDish(int restaurantId, int dishId)
    {
        var dish = _context.Dishes.FirstOrDefault(d => d.RestaurantId == restaurantId && d.Id == dishId);

        if (dish is null)
        {
            throw new NotFoundException($"Dish with id: {dishId} for restaurant with id: {restaurantId} not found");
        }

        _context.Dishes.Remove(dish);
        _context.SaveChanges();
    }
    
    public void DeleteAllDishesForRestaurant(int restaurantId)
    {
        var restaurant = GetRestaurantById(restaurantId);

        // restaurant.Dishes.RemoveAll(d => d.RestaurantId == restaurantId);
        _context.RemoveRange(restaurant.Dishes);
        _context.SaveChanges();
    }

    private Restaurant GetRestaurantById(int restaurantId)
    {
        var restaurant = _context
            .Restaurants
            .Include(r => r.Dishes)
            .FirstOrDefault(r => r.Id == restaurantId);

        if (restaurant is null)
        {
            throw new NotFoundException($"Restaurant with id: {restaurantId} not found");
        }

        return restaurant;
    }
}