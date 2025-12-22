using System.Net;
using SmartCommerce.Application.Common;
using SmartCommerce.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

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
            ValidationException ve => (HttpStatusCode.BadRequest, ve.Message),
            NotFoundException nfe => (HttpStatusCode.NotFound, nfe.Message),
            ConflictException ce => (HttpStatusCode.Conflict, ce.Message),

            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized."),
            DbUpdateException => (HttpStatusCode.Conflict, "Database update conflict."),

            _ => (HttpStatusCode.InternalServerError, "Internal server error.")
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var payload = ApiResponse<object>.Fail(message);
        await context.Response.WriteAsJsonAsync(payload);
    }
}
