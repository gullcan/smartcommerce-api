using SmartCommerce.Application.Abstractions.Repositories;
using SmartCommerce.Application.Abstractions.Services;
using SmartCommerce.Application.Common.Exceptions;
using SmartCommerce.Application.DTOs.Reviews;
using SmartCommerce.Domain.Entities;

namespace SmartCommerce.Application.Services;

public sealed class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviews;
    private readonly IProductRepository _products;

    public ReviewService(IReviewRepository reviews, IProductRepository products)
    {
        _reviews = reviews;
        _products = products;
    }

    public async Task<ReviewResponseDto> CreateAsync(Guid userId, ReviewCreateDto dto, CancellationToken ct)
    {
        var productId = Guid.Parse(dto.ProductId);

        var p = await _products.GetByIdAsync(productId, ct);
        if (p is null || p.IsDeleted)
            throw new NotFoundException("Product not found.");

        if (await _reviews.ExistsByUserAndProductAsync(userId, productId, ct))
            throw new ConflictException("You already reviewed this product.");

        var now = DateTime.UtcNow;

        var review = new Review
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            UserId = userId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        await _reviews.AddAsync(review, ct);
        await _reviews.SaveChangesAsync(ct);

        return new ReviewResponseDto(
            review.Id.ToString(),
            review.ProductId.ToString(),
            review.UserId.ToString(),
            review.Rating,
            review.Comment,
            review.CreatedAt
        );
    }

    public async Task<IReadOnlyList<ReviewResponseDto>> GetByProductAsync(Guid productId, CancellationToken ct)
    {
        var list = await _reviews.GetByProductIdAsync(productId, ct);
        return list.Select(r => new ReviewResponseDto(
            r.Id.ToString(),
            r.ProductId.ToString(),
            r.UserId.ToString(),
            r.Rating,
            r.Comment,
            r.CreatedAt
        )).ToList();
    }

    public async Task<ReviewResponseDto> UpdateAsync(Guid reviewId, Guid userId, bool isAdmin, ReviewUpdateDto dto, CancellationToken ct)
    {
        var r = await _reviews.GetByIdAsync(reviewId, ct);
        if (r is null) throw new NotFoundException("Review not found.");

        if (!isAdmin && r.UserId != userId)
            throw new UnauthorizedAccessException("Forbidden.");

        r.Rating = dto.Rating;
        r.Comment = dto.Comment;
        r.UpdatedAt = DateTime.UtcNow;

        await _reviews.SaveChangesAsync(ct);

        return new ReviewResponseDto(
            r.Id.ToString(),
            r.ProductId.ToString(),
            r.UserId.ToString(),
            r.Rating,
            r.Comment,
            r.CreatedAt
        );
    }

    public async Task DeleteAsync(Guid reviewId, Guid userId, bool isAdmin, CancellationToken ct)
    {
        var r = await _reviews.GetByIdAsync(reviewId, ct);
        if (r is null) throw new NotFoundException("Review not found.");

        if (!isAdmin && r.UserId != userId)
            throw new UnauthorizedAccessException("Forbidden.");

        r.IsDeleted = true;
        r.UpdatedAt = DateTime.UtcNow;

        await _reviews.SaveChangesAsync(ct);
    }
}
