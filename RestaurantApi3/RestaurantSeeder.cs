using Microsoft.EntityFrameworkCore;
using RestaurantApi3.Entities;

namespace RestaurantApi3;

public class RestaurantSeeder
{
    private readonly RestaurantDbContext _dbContext;
    private readonly ILogger<RestaurantSeeder> _logger;


    public RestaurantSeeder(RestaurantDbContext restaurantDbContext, ILogger<RestaurantSeeder> logger)
    {
        _dbContext = restaurantDbContext;
        _logger = logger;
    }

    public void Seed()
    {
        _logger.LogInformation(AppConstants.LoggerInformationPrefix + "Start seeding database if needed.");
        if (!_dbContext.Database.CanConnect())
        {
            _logger.LogWarning(AppConstants.LoggerWarnPrefix  + "Cannot connect to database");
            return;
        }
        
        var pendingMigrations = _dbContext.Database.GetPendingMigrations();
        if(pendingMigrations != null && pendingMigrations.Any()) {
            _logger.LogInformation(AppConstants.LoggerInformationPrefix + "Performing some migrations");
            _logger.LogDebug(AppConstants.LoggerInformationPrefix + "debug Performing some migrations");
            _logger.LogTrace(AppConstants.LoggerInformationPrefix + "trace Performing some migrations");
            _dbContext.Database.Migrate();
        }

        if (!_dbContext.Roles.Any())
        {
            var roles = GetRoles();
            _dbContext.Roles.AddRange(roles);
            _dbContext.SaveChanges();
            _logger.LogInformation(AppConstants.LoggerInformationPrefix + "Successfully seeded Roles in database");
        }


        if (!_dbContext.Restaurants.Any())
        {
            var restaurants = getRestaurants();
            _dbContext.Restaurants.AddRange(restaurants);
            _dbContext.SaveChanges();
            _logger.LogInformation(AppConstants.LoggerInformationPrefix + "Successfully seeded Restaurants in database");
        }
    }

    private IEnumerable<Restaurant> getRestaurants()
    {
        var restaurants = new List<Restaurant>()
        {
            new Restaurant()
            {
                Name = "KFC",
                Category = " Fast Food",
                Description = "KFS is an American fast food restaurant",
                HasDelivery = true,
                ContactEmail = "kfc@gmail.com",
                ContactNumber = " 453627",
                Dishes = new List<Dish>()
                {
                    new Dish()
                    {
                        Name = "Maburger",
                        Description = "Hamburgers are very good",
                        Price = 5
                    },
                    new Dish()
                    {
                        Name = "KFC Bucket",
                        Description = "Bucket full of chicken wings",
                        Price = 20
                    }
                },
                Address = new Address()
                {
                    City = "Kraków",
                    Street = "slowackiego",
                    PostalCode = "32-600"
                }
            }
        };

        return restaurants;
    }
    
    private IEnumerable<Role> GetRoles()
    {
        var roles = new List<Role>()
        {
            new() { Name = "User" },
            new() { Name = "Manager" },
            new() { Name = "Admin" },
        };

        return roles;
    }
}