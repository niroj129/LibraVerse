using System.Net;
using System.Text.Json;
using LibraVerse.DTOs.Response;

namespace LibraVerse.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An unhandled exception has occurred.");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ExceptionDto()
            {
                Status = context.Response.StatusCode,
                Error = "Something went wrong, please try again",
                Message = exception.Message,
                Timestamp = DateTime.Now
            };

            var json = JsonSerializer.Serialize(response);
            
            await context.Response.WriteAsync(json);
        }
    }
}