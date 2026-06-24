using MalakaBookFest.Application.Common;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace MalakaBookFest.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next   = next;
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
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            KeyNotFoundException               => (HttpStatusCode.NotFound, exception.Message),
            UnauthorizedAccessException        => (HttpStatusCode.Forbidden, exception.Message),
            ValidationException                => (HttpStatusCode.BadRequest, exception.Message),
            ArgumentNullException              => (HttpStatusCode.BadRequest, exception.Message),
            ArgumentException                  => (HttpStatusCode.BadRequest, exception.Message),
            InvalidOperationException          => (HttpStatusCode.BadRequest, exception.Message),
            DbUpdateConcurrencyException       => (HttpStatusCode.Conflict, "The resource was updated by another process. Please retry."),
            DbUpdateException                  => (HttpStatusCode.Conflict, "The requested change could not be saved."),
            OperationCanceledException         => (HttpStatusCode.RequestTimeout, "The request was cancelled."),
            _                                  => (HttpStatusCode.InternalServerError, exception.ToString())
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)statusCode;

        var response = ApiResponse<object>.Fail(message);
        var json     = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
