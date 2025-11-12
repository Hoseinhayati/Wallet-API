using System.Net;
using System.Text.Json;
using DigitalWallet.Wallet.Application.Common.Exceptions;
using DigitalWallet.Wallet.Domain.Exceptions;

namespace DigitalWallet.Wallet.API.Middleware;

/// <summary>
/// Standard error response model
/// </summary>
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public object? Errors { get; set; }
    public string? StackTrace { get; set; }
}

/// <summary>
/// Global exception handler middleware to catch and format all exceptions
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ValidationException validationException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "خطای اعتبارسنجی رخ داده است.",
                Errors = validationException.Errors,
                Type = "ValidationError"
            },
            WalletDomainException domainException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = domainException.Message,
                Type = "DomainError"
            },
            _ => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = _environment.IsDevelopment()
                    ? exception.Message
                    : "خطای سرور رخ داده است. لطفا دوباره تلاش کنید.",
                Type = "ServerError",
                // Only include stack trace in development
                StackTrace = _environment.IsDevelopment() ? exception.StackTrace : null
            }
        };

        context.Response.StatusCode = response.StatusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // For Farsi characters
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

/// <summary>
/// Extension method to register the global exception handler middleware
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}