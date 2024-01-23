using RestaurantApi3.Exceptions;

namespace RestaurantApi3.Middlewares;

public class ErrorHandlingMiddleware: IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly ConnectionStringsSettings _connectionStringsSettings;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware>logger, ConnectionStringsSettings connectionStringsSettings)
    {
        _logger = logger;
        _connectionStringsSettings = connectionStringsSettings;

    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (BadRequestException badRequestException)
        {
            _logger.LogError(badRequestException, AppConstants.LoggerErrorPrefix + badRequestException.Message);
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(badRequestException.Message);
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException, AppConstants.LoggerErrorPrefix + notFoundException.Message);
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(notFoundException.Message);
        }
        catch (ForbidException forbidException)
        {
            _logger.LogError(forbidException, AppConstants.LoggerErrorPrefix + forbidException.Message);
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync(forbidException.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{AppConstants.LoggerErrorPrefix} Something went wrong: {e.Message}");
            _logger.LogWarning($"{AppConstants.LoggerWarnPrefix} Something went wrong: {e.Message}");
            _logger.LogInformation($"{AppConstants.LoggerInformationPrefix} Something went wrong: {e.Message}");
            context.Response.StatusCode = 500;
            
            await context.Response.WriteAsync($"Something went wrong: {e.Message}");
        }
    }
}