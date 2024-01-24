using System.Reflection;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RestaurantApi3;
using RestaurantApi3.Entities;
using RestaurantApi3.Services;
using NLog;
using NLog.Web;
using RestaurantApi3.Authorization.handlers;
using RestaurantApi3.Authorization.requirements;
using RestaurantApi3.Dtos;
using RestaurantApi3.Dtos.Validators;
using RestaurantApi3.Middlewares;

// https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-6
// Early init of NLog to allow startup and exception logging, before host is built
// var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

var nlog = NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();


var builder = WebApplication.CreateBuilder(args);

// https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-6
builder.Logging.ClearProviders();
builder.Host.UseNLog();


// Add services to the container.

builder.Services.AddControllers().AddFluentValidation();

builder.Services.AddDbContext<RestaurantDbContext>();
builder.Services.AddScoped<RestaurantSeeder>();

builder.Services.AddScoped<IRestaurantService, RestaurantService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddScoped<ErrorHandlingMiddleware>();

AuthenticationSettings authSettings = new AuthenticationSettings();
builder.Configuration.GetSection("Authentication").Bind(authSettings);
builder.Services.AddSingleton<AuthenticationSettings>(authSettings);

ConnectionStringsSettings connectionStringsSettings = new ConnectionStringsSettings();
builder.Configuration.GetSection("ConnectionStrings").Bind(connectionStringsSettings);
// builder.Services.AddSingleton<ConnectionStringsSettings>(connectionStringsSettings);
builder.Services.AddSingleton(connectionStringsSettings);

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";

}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = authSettings.JwtIssuer,
        ValidAudience = authSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.JwtKey)),
        // ValidateIssuer = true,
        // ValidateAudience = true,
        // ValidateLifetime = true,
        // ValidateIssuerSigningKey = true
        // above commented code was presented in another tutorial
    };
});

// authorization
builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CreatedMultipleRestaurantsHandler>();


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HasNationality", 
        policyBuilder => policyBuilder.RequireClaim("Nationality", "German"));
    
    options.AddPolicy("AtLeast20", 
        policyBuilder => policyBuilder.AddRequirements(new MinimumAgeRequirement(20)));
    
    options.AddPolicy("CreateAtLeast2Restaurants", policyBuilder =>
        policyBuilder.AddRequirements(new CreatedMultipleRestaurantsRequirement(2)));
});





// poniższego nie robimy jesli mamy autoryzacje z dostepem do zasobu
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("ResourceOwner", policyBuilder =>
//         policyBuilder.AddRequirements(new ResourceOperationRequirement()));
//     // policyBuilder.Requirements.Add(new MinimumAgeRequirement(25)));
// });


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendClient", configurePolicy =>
    {
        configurePolicy
            .AllowAnyHeader()
            // .WithHeaders(...)
            .AllowAnyMethod()
            // .AllowAnyOrigin()
            .WithOrigins(builder.Configuration["AllowedOrigins"]);
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseMiddleware<ErrorHandlingMiddleware>();

//does not work - would work for singleton
// app.Services.GetService<RestaurantSeeder>()!.Seed();

var scope = app.Services.CreateScope();
var dbSeeder = scope.ServiceProvider.GetRequiredService<RestaurantSeeder>();
dbSeeder.Seed();

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();