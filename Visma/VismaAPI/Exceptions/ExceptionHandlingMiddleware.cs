using System.Net;
using System.Text.Json;

namespace VismaAPI.Exceptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception occurred");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        HttpStatusCode status;
        object response;

        if (exception is Exceptions.ValidationException validationException)
        {
            status = HttpStatusCode.BadRequest;
            response = new
            {
                status = (int)status,
                error = "Validation Failed",
                details = validationException.Errors
            };
        }
        else
        {
            status = HttpStatusCode.InternalServerError;
            response = new
            {
                status = (int)status,
                error = "Internal Server Error",
                details = exception.Message
            };
        }

        context.Response.StatusCode = (int)status;
        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}
