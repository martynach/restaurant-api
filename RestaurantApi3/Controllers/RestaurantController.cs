using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApi3.Dtos;
using RestaurantApi3.Entities;
using RestaurantApi3.Services;

namespace RestaurantApi3.Controllers;

[ApiController]
[Route("api/restaurant")]
[Authorize]
public class RestaurantController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;
    private readonly ILogger<RestaurantController> _logger;

    public RestaurantController(IRestaurantService restaurantService, ILogger<RestaurantController> logger)
    {
        _restaurantService = restaurantService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    // [Authorize(Policy = "CreateAtLeast2Restaurants")]
    public ActionResult<PagedResult<Restaurant>> GetAll([FromQuery] RestaurantQuery query)
    {
        _logger.LogInformation(AppConstants.LoggerInformationPrefix + "Getting all the restaurants");
        var pagedResult = _restaurantService.GetAll(query);
        return Ok(pagedResult);
    }

    [Authorize(Policy = "HasNationality")]
    [Authorize(Policy = "AtLeast20")]
    [HttpGet("{id}")]
    public ActionResult<Restaurant> GetById([FromRoute] int id)
    {
        var restaurantDto = _restaurantService.GetById(id);
        return Ok(restaurantDto);
    }

    [HttpPost]
    // [Authorize(Roles = "Admin")]
    // [Authorize(Roles = "Manager")]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
    {
        // if (!ModelState.IsValid)
        // {
        //     return BadRequest(ModelState);
        // }
        // the above code is done by [ApiController] annotation
        
        
        var id = _restaurantService.CreateRestaurant(dto);


        return Created($"/api/restaurant/{id}", null);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpDelete("{id}")]
    public ActionResult DeleteRestaurant([FromRoute] int id)
    {
        _restaurantService.DeleteRestaurant(id);
        return NoContent();
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id}")]
    public ActionResult UpdateRestaurant([FromRoute] int id, [FromBody] UpdateRestaurantDto updateRestaurantDto)
    {
        // [apicontroller] does below validation for us 
        // if (!ModelState.IsValid)
        // {
        //     return BadRequest(ModelState);
        // }

        _restaurantService.UpdateRestaurant(id, updateRestaurantDto);
        return Ok();
    }
}