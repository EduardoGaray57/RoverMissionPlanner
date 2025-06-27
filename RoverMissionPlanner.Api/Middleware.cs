using System.Net;
using System.Text.Json;

namespace RoverMissionPlanner.Api.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            object response;
            int statusCode;

            switch (exception)
            {
                case InvalidOperationException:
                    statusCode = (int)HttpStatusCode.Conflict;
                    response = new { message = exception.Message };
                    break;
                case ArgumentNullException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new { message = exception.Message };
                    break;
                case ArgumentException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new { message = exception.Message };
                    break;
                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    response = new { 
                        message = "An error occurred while processing your request",
                        details = exception.Message 
                    };
                    break;
            }

            context.Response.StatusCode = statusCode;
            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }

    public static class GlobalExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        }
    }
}