using System.ComponentModel.DataAnnotations;

namespace RestaurantApi3.Dtos
{
    public class LoginUserDto
    {
        public string Email { get; set; }
        
        public string Password { get; set; }

    }
}