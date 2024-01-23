
using RestaurantApi3.Dtos;

namespace RestaurantApi3.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto dto);
        string GenerateJwt(LoginUserDto dto);
    }
}