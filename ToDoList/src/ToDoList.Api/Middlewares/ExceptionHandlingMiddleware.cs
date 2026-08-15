using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Application.Exceptions;

namespace ToDoList.Api.Middlewares;

/// <summary>
/// Catches unhandled exceptions and maps the application's custom exceptions
/// to consistent RFC-7807 ProblemDetails responses with the right status code.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "One or more validation errors occurred."),
            EmailAlreadyExistsException => (StatusCodes.Status409Conflict, "Conflict."),
            NotFoundException => (StatusCodes.Status404NotFound, "Resource not found."),
            UserNotFoundException => (StatusCodes.Status404NotFound, "User not found."),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized."),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception processing {Method} {Path}",
                context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning("Handled {ExceptionType}: {Message}",
                exception.GetType().Name, exception.Message);
        }

        ProblemDetails problem;
        if (exception is ValidationException validationException && validationException.Errors.Count > 0)
        {
            problem = new ValidationProblemDetails(
                validationException.Errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
            {
                Status = statusCode,
                Title = title,
                Instance = context.Request.Path
            };
        }
        else
        {
            problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = statusCode == StatusCodes.Status500InternalServerError && !_environment.IsDevelopment()
                    ? "An unexpected error occurred. Please try again later."
                    : exception.Message,
                Instance = context.Request.Path
            };
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, problem.GetType(), options));
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
