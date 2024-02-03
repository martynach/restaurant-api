using RestaurantApi3.Exceptions;

namespace RestaurantApi3.Middlewares;

public class ErrorHandlingMiddleware: IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware>logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (BadRequestException badRequestException)
        {
            _logger.LogError(badRequestException, AppConstants.LoggerPrefix + badRequestException.Message);
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(badRequestException.Message);
        }
        catch (NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException, AppConstants.LoggerPrefix + notFoundException.Message);
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(notFoundException.Message);
        }
        catch (ForbidException forbidException)
        {
            _logger.LogError(forbidException, AppConstants.LoggerPrefix + forbidException.Message);
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync(forbidException.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{AppConstants.LoggerPrefix} Something went wrong: {e.Message}");
            _logger.LogWarning($"{AppConstants.LoggerPrefix} Something went wrong: {e.Message}");
            context.Response.StatusCode = 500;
            
            await context.Response.WriteAsync($"Something went wrong: {e.Message}");
        }
    }
}