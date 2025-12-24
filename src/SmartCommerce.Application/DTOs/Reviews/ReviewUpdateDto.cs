using SmartCommerce.Application.Common.Validation;

namespace SmartCommerce.Application.DTOs.Reviews;

public sealed record ReviewUpdateDto(int Rating, string? Comment)
{
    public Dictionary<string, string[]> Validate()
    {
        if (Rating < 1 || Rating > 5)
            return ValidationErrors.Single(nameof(Rating), "Rating must be between 1 and 5.");

        if (Comment is not null && Comment.Length > 500)
            return ValidationErrors.Single(nameof(Comment), "Comment must be at most 500 characters.");

        return new();
    }
}
