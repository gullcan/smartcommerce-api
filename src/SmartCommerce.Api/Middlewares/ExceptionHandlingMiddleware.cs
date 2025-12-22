using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartCommerce.Application.Common;
using SmartCommerce.Application.Common.Exceptions;

namespace SmartCommerce.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
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
            _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);
            await WriteErrorResponseAsync(context, ex);
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message) = ex switch
        {
            // App exceptions
            ValidationException ve => (HttpStatusCode.BadRequest, ve.Message),
            NotFoundException nfe => (HttpStatusCode.NotFound, nfe.Message),
            ConflictException ce => (HttpStatusCode.Conflict, ce.Message),

            // Request/JSON problems (önceden 500'e düşebiliyordu)
            JsonException => (HttpStatusCode.BadRequest, "Invalid JSON payload."),
            BadHttpRequestException => (HttpStatusCode.BadRequest, "Bad request."),
            FormatException => (HttpStatusCode.BadRequest, "Invalid request format."),

            // Auth
            UnauthorizedAccessException uae => (HttpStatusCode.Unauthorized, uae.Message),

            // DB
            DbUpdateException => (HttpStatusCode.Conflict, "Database update conflict."),

            _ => (HttpStatusCode.InternalServerError, "Internal server error.")
        };

        if (context.Response.HasStarted) return;

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var payload = ApiResponse<object>.Fail(message);
        await context.Response.WriteAsJsonAsync(payload);
    }
}
