using Microsoft.AspNetCore.Mvc;
using RestaurantApi3.Dtos;
using RestaurantApi3.Services;

namespace RestaurantApi3.Controllers;

[ApiController]
[Route("api/restaurant/{restaurantId}/dish")]
public class DishController: ControllerBase
{
    private readonly ILogger<DishController> _logger;
    private readonly IDishService _dishService;

    public DishController(ILogger<DishController> logger, IDishService dishService)
    {
        _logger = logger;
        _dishService = dishService;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<DishDto>> GetDishes([FromRoute] int restaurantId)
    {
        var dishes = _dishService.GetAll(restaurantId);
        return Ok(dishes);
    }

    [HttpGet("{dishId}")]
    public ActionResult<DishDto> GetById([FromRoute] int restaurantId, [FromRoute] int dishId)
    {
        var dish = _dishService.GetDishById(restaurantId, dishId);
        return Ok(dish);
    }

    [HttpPost]
    public ActionResult CreateDish([FromRoute] int restaurantId, [FromBody] CreateDishDto dish)
    {
        var newDishId = _dishService.CreateDish(restaurantId, dish);
        return Created($"/api/restaurant/{restaurantId}/dish/{newDishId}", null);
    }



    [HttpDelete("/{dishId}")]
    public ActionResult DeleteDish([FromRoute] int restaurantId, [FromRoute] int dishId)
    {
        _dishService.DeleteDish(restaurantId, dishId);
        return Ok();
    }
    
    [HttpDelete]
    public ActionResult DeleteAll([FromRoute] int restaurantId)
    {
        _dishService.DeleteAllDishesForRestaurant(restaurantId);
        return NoContent();
    }
    
}