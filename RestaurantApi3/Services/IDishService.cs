using RestaurantApi3.Dtos;

namespace RestaurantApi3.Services;

public interface IDishService
{
    public IEnumerable<DishDto> GetAll(int restaurantId);
    public DishDto GetDishById(int restaurantId, int dishId);
    public int CreateDish(int restaurantId, CreateDishDto dishDto);
    public void DeleteDish(int restaurantId, int dishId);
    public void DeleteAllDishesForRestaurant(int restaurantId);
}