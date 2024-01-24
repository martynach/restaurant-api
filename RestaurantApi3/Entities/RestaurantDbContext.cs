using Microsoft.EntityFrameworkCore;

namespace RestaurantApi3.Entities;

public class RestaurantDbContext: DbContext
{
    private readonly ConnectionStringsSettings _connectionStringsSettings;
    private readonly ILogger<RestaurantDbContext> _logger;
    

    public RestaurantDbContext(ConnectionStringsSettings connectionStringsSettings, ILogger<RestaurantDbContext> logger)
    {
        _connectionStringsSettings = connectionStringsSettings;
        _logger = logger;
    }

    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<Address> Addresses { get; set; }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired();

        modelBuilder.Entity<Role>()
            .Property(r => r.Name)
            .IsRequired();
        
        modelBuilder.Entity<Restaurant>()
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<Dish>()
            .Property(c => c.Name)
            .IsRequired();

        modelBuilder.Entity<Address>()
            .Property(c => c.City)
            .IsRequired()
            .HasMaxLength(50);
        
        modelBuilder.Entity<Address>()
            .Property(p => p.Street)
            .IsRequired()
            .HasMaxLength(50);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _connectionStringsSettings.RestaurantDbConnection;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = Environment.GetEnvironmentVariable("RestaurantDbConnection");
        }
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (String.Equals(environment?.ToLower(), "development") || String.Equals(environment?.ToLower(), "private"))
        {
            _logger.LogInformation($"{AppConstants.LoggerInformationPrefix} Using local POSTGRES DB");
            _logger.LogWarning($"{AppConstants.LoggerWarnPrefix} Using local POSTGRES DB");
            optionsBuilder.UseNpgsql(connectionString);
        }
        else
        {
            _logger.LogInformation($"{AppConstants.LoggerInformationPrefix} Using azure sql server");
            _logger.LogWarning($"{AppConstants.LoggerWarnPrefix} Using azure sql server");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

}