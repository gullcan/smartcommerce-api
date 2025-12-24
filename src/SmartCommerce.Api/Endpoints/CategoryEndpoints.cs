using SmartCommerce.Application.Abstractions.Services;
using SmartCommerce.Application.Common;
using SmartCommerce.Application.DTOs.Categories;

namespace SmartCommerce.Api.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/categories")
            .WithTags("Categories");

        // GET /categories
        group.MapGet("/", async (ICategoryService service, CancellationToken ct) =>
        {
            var data = await service.GetAllAsync(ct);
            return Results.Ok(ApiResponse<IReadOnlyList<CategoryResponseDto>>.Ok(data, "Categories retrieved."));
        });

        // GET /categories/{id}
        group.MapGet("/{id}", async (string id, ICategoryService service, CancellationToken ct) =>
        {
            if (!Guid.TryParse(id, out var categoryId))
                return Results.BadRequest(ApiResponse<object>.Fail("Invalid id format."));

            var data = await service.GetByIdAsync(categoryId, ct);
            return Results.Ok(ApiResponse<CategoryResponseDto>.Ok(data, "Category retrieved."));
        });

        // POST /categories (Admin only)
        group.MapPost("/", async (CategoryCreateDto dto, ICategoryService service, CancellationToken ct) =>
        {
            var errors = dto.Validate();
            if (errors.Count > 0)
                return Results.BadRequest(ApiResponse<object>.Fail("Validation failed.", errors));

            var created = await service.CreateAsync(dto, ct);

            return Results.Created($"/categories/{created.Id}",
                ApiResponse<CategoryResponseDto>.Ok(created, "Category created."));
        })
        .RequireAuthorization("AdminOnly");

        // PUT /categories/{id} (Admin only)
        group.MapPut("/{id}", async (string id, CategoryUpdateDto dto, ICategoryService service, CancellationToken ct) =>
        {
            if (!Guid.TryParse(id, out var categoryId))
                return Results.BadRequest(ApiResponse<object>.Fail("Invalid id format."));

            var errors = dto.Validate();
            if (errors.Count > 0)
                return Results.BadRequest(ApiResponse<object>.Fail("Validation failed.", errors));

            var updated = await service.UpdateAsync(categoryId, dto, ct);
            return Results.Ok(ApiResponse<CategoryResponseDto>.Ok(updated, "Category updated."));
        })
        .RequireAuthorization("AdminOnly");

        // DELETE /categories/{id} (Admin only)
        group.MapDelete("/{id}", async (string id, ICategoryService service, CancellationToken ct) =>
        {
            if (!Guid.TryParse(id, out var categoryId))
                return Results.BadRequest(ApiResponse<object>.Fail("Invalid id format."));

            await service.DeleteAsync(categoryId, ct);
            return Results.Ok(ApiResponse<object>.OkEmpty("Category deleted."));
        })
        .RequireAuthorization("AdminOnly");

        return app;
    }
}
