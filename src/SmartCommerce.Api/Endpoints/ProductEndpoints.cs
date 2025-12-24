using SmartCommerce.Application.Abstractions.Services;
using SmartCommerce.Application.Common;
using SmartCommerce.Application.DTOs.Products;

namespace SmartCommerce.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products")
            .WithTags("Products");

        // GET /products
        group.MapGet("/", async (IProductService service, CancellationToken ct) =>
        {
            var data = await service.GetAllAsync(ct);
            return Results.Ok(ApiResponse<IReadOnlyList<ProductResponseDto>>.Ok(data, "Products retrieved."));
        });

        // GET /products/{id}
        group.MapGet("/{id}", async (string id, IProductService service, CancellationToken ct) =>
        {
            if (!Guid.TryParse(id, out var productId))
                return Results.BadRequest(ApiResponse<object>.Fail("Invalid id format."));

            var data = await service.GetByIdAsync(productId, ct);
            return Results.Ok(ApiResponse<ProductResponseDto>.Ok(data, "Product retrieved."));
        });

        // POST /products (Admin only)
        group.MapPost("/", async (ProductCreateDto dto, IProductService service, CancellationToken ct) =>
        {
            var errors = dto.Validate();
if (errors.Count > 0)
    return Results.BadRequest(ApiResponse<object>.Fail("Validation failed.", errors));

            var created = await service.CreateAsync(dto, ct);

            return Results.Created($"/products/{created.Id}",
                ApiResponse<ProductResponseDto>.Ok(created, "Product created."));
        })
        .RequireAuthorization("AdminOnly");

        // PUT /products/{id} (Admin only)
        group.MapPut("/{id}", async (string id, ProductUpdateDto dto, IProductService service, CancellationToken ct) =>
        {
            if (!Guid.TryParse(id, out var productId))
                return Results.BadRequest(ApiResponse<object>.Fail("Invalid id format."));

            var updated = await service.UpdateAsync(productId, dto, ct);
            return Results.Ok(ApiResponse<ProductResponseDto>.Ok(updated, "Product updated."));
        })
        .RequireAuthorization("AdminOnly");

        // DELETE /products/{id} (Admin only)
        group.MapDelete("/{id}", async (string id, IProductService service, CancellationToken ct) =>
        {
            if (!Guid.TryParse(id, out var productId))
                return Results.BadRequest(ApiResponse<object>.Fail("Invalid id format."));

            await service.DeleteAsync(productId, ct);
            return Results.Ok(ApiResponse<object>.OkEmpty("Product deleted."));
        })
        .RequireAuthorization("AdminOnly");

        return app;
    }
}
