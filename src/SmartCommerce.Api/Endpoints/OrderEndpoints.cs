using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SmartCommerce.Application.Abstractions.Services;
using SmartCommerce.Application.Common;
using SmartCommerce.Application.DTOs.Orders;

namespace SmartCommerce.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders").WithTags("Orders");

        static Guid GetUserId(ClaimsPrincipal user)
        {
            var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                      user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (sub is null || !Guid.TryParse(sub, out var id))
                throw new UnauthorizedAccessException("Invalid token subject.");

            return id;
        }

        group.MapGet("/", async (HttpContext http, IOrderService service, CancellationToken ct) =>
        {
            var isAdmin = http.User.IsInRole("Admin");
            var data = isAdmin
                ? await service.GetAllAsync(ct)
                : await service.GetMineAsync(GetUserId(http.User), ct);

            return Results.Ok(ApiResponse<IReadOnlyList<OrderResponseDto>>.Ok(data, "Orders retrieved."));
        }).RequireAuthorization();

        group.MapGet("/{id}", async (string id, HttpContext http, IOrderService service, CancellationToken ct) =>
        {
            if (!Guid.TryParse(id, out var orderId))
                return Results.BadRequest(ApiResponse<object>.Fail("Invalid id format."));

            var order = await service.GetByIdAsync(orderId, ct);

            var isAdmin = http.User.IsInRole("Admin");
            var me = GetUserId(http.User);
            if (!isAdmin && order.UserId != me.ToString())
                return Results.Forbid();

            return Results.Ok(ApiResponse<OrderResponseDto>.Ok(order, "Order retrieved."));
        }).RequireAuthorization();

        group.MapPost("/", async (OrderCreateDto dto, HttpContext http, IOrderService service, CancellationToken ct) =>
        {
            var errors = dto.Validate();
            if (errors.Count > 0)
                return Results.BadRequest(ApiResponse<object>.Fail("Validation failed.", errors));

            var created = await service.CreateAsync(GetUserId(http.User), dto, ct);
            return Results.Created($"/orders/{created.Id}", ApiResponse<OrderResponseDto>.Ok(created, "Order created."));
        }).RequireAuthorization();

        group.MapPut("/{id}/status", async (string id, OrderUpdateStatusDto dto, IOrderService service, CancellationToken ct) =>
        {
            if (!Guid.TryParse(id, out var orderId))
                return Results.BadRequest(ApiResponse<object>.Fail("Invalid id format."));

            var errors = dto.Validate();
            if (errors.Count > 0)
                return Results.BadRequest(ApiResponse<object>.Fail("Validation failed.", errors));

            var updated = await service.UpdateStatusAsync(orderId, dto.Status, ct);
            return Results.Ok(ApiResponse<OrderResponseDto>.Ok(updated, "Order status updated."));
        }).RequireAuthorization("AdminOnly");

        group.MapDelete("/{id}", async (string id, IOrderService service, CancellationToken ct) =>
        {
            if (!Guid.TryParse(id, out var orderId))
                return Results.BadRequest(ApiResponse<object>.Fail("Invalid id format."));

            await service.DeleteAsync(orderId, ct);
            return Results.Ok(ApiResponse<object>.OkEmpty("Order deleted."));
        }).RequireAuthorization("AdminOnly");

        return app;
    }
}
