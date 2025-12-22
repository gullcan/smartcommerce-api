using SmartCommerce.Application.Abstractions.Services;
using SmartCommerce.Application.Common;
using SmartCommerce.Application.Common.Exceptions;
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

            try
            {
                var data = await service.GetByIdAsync(categoryId, ct);
                return Results.Ok(ApiResponse<CategoryResponseDto>.Ok(data, "Category retrieved."));
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(ApiResponse<object>.Fail(ex.Message));
            }
        });

        // POST /categories
        group.MapPost("/", async (CategoryCreateDto dto, ICategoryService service, CancellationToken ct) =>
        {
            try
            {
                var created = await service.CreateAsync(dto, ct);
                return Results.Created($"/categories/{created.Id}",
                    ApiResponse<CategoryResponseDto>.Ok(created, "Category created."));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (ConflictException ex)
            {
                return Results.Conflict(ApiResponse<object>.Fail(ex.Message));
            }
            catch
            {
                return Results.Json(ApiResponse<object>.Fail("Internal server error."), statusCode: 500);
            }
        });

        // PUT /categories/{id}
        group.MapPut("/{id}", async (string id, CategoryUpdateDto dto, ICategoryService service, CancellationToken ct) =>
        {
            if (!Guid.TryParse(id, out var categoryId))
                return Results.BadRequest(ApiResponse<object>.Fail("Invalid id format."));

            try
            {
                var updated = await service.UpdateAsync(categoryId, dto, ct);
                return Results.Ok(ApiResponse<CategoryResponseDto>.Ok(updated, "Category updated."));
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (ConflictException ex)
            {
                return Results.Conflict(ApiResponse<object>.Fail(ex.Message));
            }
            catch
            {
                return Results.Json(ApiResponse<object>.Fail("Internal server error."), statusCode: 500);
            }
        });

        // DELETE /categories/{id} (Soft delete)
        group.MapDelete("/{id}", async (string id, ICategoryService service, CancellationToken ct) =>
        {
            if (!Guid.TryParse(id, out var categoryId))
                return Results.BadRequest(ApiResponse<object>.Fail("Invalid id format."));

            try
            {
                await service.DeleteAsync(categoryId, ct);
                return Results.Ok(ApiResponse<object>.OkEmpty("Category deleted."));
            }
            catch (NotFoundException ex)
            {
                return Results.NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (ConflictException ex)
            {
                return Results.Conflict(ApiResponse<object>.Fail(ex.Message));
            }
            catch
            {
                return Results.Json(ApiResponse<object>.Fail("Internal server error."), statusCode: 500);
            }
        });

        return app;
    }
}
