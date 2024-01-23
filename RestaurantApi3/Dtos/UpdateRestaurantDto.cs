
using System.ComponentModel.DataAnnotations;

namespace RestaurantApi3.Dtos;

public class UpdateRestaurantDto
{
    [MaxLength(50)]
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? HasDelivery { get; set; }
}