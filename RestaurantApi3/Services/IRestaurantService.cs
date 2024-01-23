using System.Security.Claims;
using RestaurantApi3.Dtos;

namespace RestaurantApi3.Services;

public interface IRestaurantService
{

    PagedResult<RestaurantDto> GetAll(RestaurantQuery query);
    RestaurantDto GetById(int id);
    
    public int CreateRestaurant(CreateRestaurantDto dto);

    public void DeleteRestaurant(int id);
    public void UpdateRestaurant(int id, UpdateRestaurantDto updateDto);
}