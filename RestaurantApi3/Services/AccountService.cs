using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantApi3.Dtos;
using RestaurantApi3.Entities;
using RestaurantApi3.Exceptions;

namespace RestaurantApi3.Services
{
    public class AccountService : IAccountService
    {
        private RestaurantDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;

        public AccountService(RestaurantDbContext context, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
        }
        

        public void RegisterUser(RegisterUserDto dto)
        {
            TimeZoneInfo pacificZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            
            var datetime = new DateTime(dto.DateOfBirth!.Value.Year, dto.DateOfBirth.Value.Month, dto.DateOfBirth.Value.Day);
            var date = TimeZoneInfo.ConvertTimeToUtc(datetime, pacificZone);
            var user = new User()
            {
                Email = dto.Email,
                Nationality = dto.Nationality,
                DateOfBirth = date,
                // DateOfBirth = dto.DateOfBirth,
                RoleId = dto.RoleId,
            };

            Console.WriteLine($"---------------------------------");
            Console.WriteLine($"---------------------------------");
            Console.WriteLine($"dto.dateofbirth: {dto.DateOfBirth}");
            Console.WriteLine($"user.dateofbirth: {user.DateOfBirth}");
            Console.WriteLine($"---------------------------------");
            Console.WriteLine($"---------------------------------");

            var hash = _passwordHasher.HashPassword(user, dto.Password);
            user.PasswordHash = hash;

            _context.Users.Add(user);
            _context.SaveChanges();
        }


        public string GenerateJwt(LoginUserDto dto)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == dto.Email);
            if (user is null)
            {
                throw new BadRequestException("Wrong email");
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Wrong password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
                new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd"))
            };

            if (!string.IsNullOrEmpty(user.Nationality))
            {
                claims.Add(new Claim("Nationality", $"{user.Nationality}"));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            string sToken = tokenHandler.WriteToken(token);
            return sToken;
        }
    }
}
