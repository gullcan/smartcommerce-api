using SmartCommerce.Application.Abstractions.Services;
using SmartCommerce.Application.Common;
using SmartCommerce.Application.DTOs.Auth;

namespace SmartCommerce.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
            .WithTags("Auth");

        // POST /auth/register
        group.MapPost("/register", async (AuthRegisterDto dto, IAuthService service, CancellationToken ct) =>
        {
            var result = await service.RegisterAsync(dto, ct);
            return Results.Created($"/users/{result.UserId}", ApiResponse<AuthResponseDto>.Ok(result, "Registered."));
        })
        .AllowAnonymous();

        // POST /auth/login
        group.MapPost("/login", async (AuthLoginDto dto, IAuthService service, CancellationToken ct) =>
        {
            var result = await service.LoginAsync(dto, ct);
            return Results.Ok(ApiResponse<AuthResponseDto>.Ok(result, "Logged in."));
        })
        .AllowAnonymous();

        return app;
    }
}
