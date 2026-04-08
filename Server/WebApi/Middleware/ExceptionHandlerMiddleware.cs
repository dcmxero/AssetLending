using System.Net;
using System.Text.Json;
using DTOs.Common;

namespace WebApi.Middleware;

/// <summary>
/// Global exception handler middleware that catches unhandled exceptions
/// and returns a standardized error response.
/// </summary>
public sealed class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    /// <summary>
    /// Invokes the middleware pipeline, catching any unhandled exceptions.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred while processing {Method} {Path}",
                context.Request.Method, context.Request.Path);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                Error = "An internal server error occurred.",
                Details = null
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
