using SmartCommerce.Application.DTOs.Reviews;

namespace SmartCommerce.Application.Abstractions.Services;

public interface IReviewService
{
    Task<ReviewResponseDto> CreateAsync(Guid userId, ReviewCreateDto dto, CancellationToken ct);
    Task<IReadOnlyList<ReviewResponseDto>> GetByProductAsync(Guid productId, CancellationToken ct);
    Task<ReviewResponseDto> UpdateAsync(Guid reviewId, Guid userId, bool isAdmin, ReviewUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(Guid reviewId, Guid userId, bool isAdmin, CancellationToken ct);
}
