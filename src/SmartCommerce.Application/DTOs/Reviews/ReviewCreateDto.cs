using SmartCommerce.Application.Common.Validation;

namespace SmartCommerce.Application.DTOs.Reviews;

public sealed record ReviewCreateDto(string ProductId, int Rating, string? Comment)
{
    public Dictionary<string, string[]> Validate()
    {
        if (!Guid.TryParse(ProductId, out _))
            return ValidationErrors.Single(nameof(ProductId), "ProductId is invalid.");

        if (Rating < 1 || Rating > 5)
            return ValidationErrors.Single(nameof(Rating), "Rating must be between 1 and 5.");

        if (Comment is not null && Comment.Length > 500)
            return ValidationErrors.Single(nameof(Comment), "Comment must be at most 500 characters.");

        return new();
    }
}
