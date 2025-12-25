using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SmartCommerce.Application.Abstractions.Services;
using SmartCommerce.Application.Common;
using SmartCommerce.Application.DTOs.Reviews;

namespace SmartCommerce.Api.Endpoints;

public static class ReviewEndpoints
{
    public static IEndpointRouteBuilder MapReviewEndpoints(this IEndpointRouteBuilder app)
    {
    
        var restful = app.MapGroup("/products/{productId}/reviews").WithTags("Reviews");

        static Guid GetUserId(ClaimsPrincipal user)
        {
            var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                      user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (sub is null || !Guid.TryParse(sub, out var id))
                throw new UnauthorizedAccessException("Invalid token subject.");

            return id;
        }


        // RESTful: GET /products/{productId}/reviews
        restful.MapGet("", async (string productId, IReviewService service, CancellationToken ct) =>
        {
            if (!Guid.TryParse(productId, out var pid))
                return Results.BadRequest(ApiResponse<object>.Fail("Invalid productId."));

            var data = await service.GetByProductAsync(pid, ct);
            return Results.Ok(ApiResponse<IReadOnlyList<ReviewResponseDto>>.Ok(data, "Reviews retrieved."));
        });

        
        // RESTful: POST /products/{productId}/reviews
        // Not: Bu endpoint body'deki dto'yu aynen kullanır. (Body'de productId alanı varsa zaten service onu kullanacaktır.)
        restful.MapPost("", async (string productId, ReviewCreateDto dto, HttpContext http, IReviewService service, CancellationToken ct) =>
        {
            if (!Guid.TryParse(productId, out var pid))
                return Results.BadRequest(ApiResponse<object>.Fail("Invalid productId."));

            dto = dto with { ProductId = pid.ToString() };

            var errors = dto.Validate();
            
            if (errors.Count > 0)
                return Results.BadRequest(ApiResponse<object>.Fail("Validation failed.", errors));

            var created = await service.CreateAsync(GetUserId(http.User), dto, ct);
            return Results.Created($"/products/{pid}/reviews/{created.Id}",
                ApiResponse<ReviewResponseDto>.Ok(created, "Review created."));
        }).RequireAuthorization();


        // RESTful: PUT /products/{productId}/reviews/{id}
        restful.MapPut("/{id}", async (string id, ReviewUpdateDto dto, HttpContext http, IReviewService service, CancellationToken ct) =>
        {
            if (!Guid.TryParse(id, out var rid))
                return Results.BadRequest(ApiResponse<object>.Fail("Invalid id format."));

            var errors = dto.Validate();
            if (errors.Count > 0)
                return Results.BadRequest(ApiResponse<object>.Fail("Validation failed.", errors));

            var updated = await service.UpdateAsync(rid, GetUserId(http.User), http.User.IsInRole("Admin"), dto, ct);
            return Results.Ok(ApiResponse<ReviewResponseDto>.Ok(updated, "Review updated."));
        }).RequireAuthorization();
       

        // RESTful: DELETE /products/{productId}/reviews/{id}
        restful.MapDelete("/{id}", async (string id, HttpContext http, IReviewService service, CancellationToken ct) =>
        {
            if (!Guid.TryParse(id, out var rid))
                return Results.BadRequest(ApiResponse<object>.Fail("Invalid id format."));

            await service.DeleteAsync(rid, GetUserId(http.User), http.User.IsInRole("Admin"), ct);
            return Results.Ok(ApiResponse<object>.OkEmpty("Review deleted."));
        }).RequireAuthorization();

        return app;
    }
}
