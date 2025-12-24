using SmartCommerce.Application.Common.Validation;

namespace SmartCommerce.Application.DTOs.Categories;

public sealed record CategoryUpdateDto(string Name, string? Description)
{
    public Dictionary<string, string[]> Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return ValidationErrors.Single(nameof(Name), "Name is required.");

        if (Name.Trim().Length < 2)
            return ValidationErrors.Single(nameof(Name), "Name must be at least 2 characters.");

        if (Name.Trim().Length > 50)
            return ValidationErrors.Single(nameof(Name), "Name must be at most 50 characters.");

        if (Description is not null && Description.Length > 200)
            return ValidationErrors.Single(nameof(Description), "Description must be at most 200 characters.");

        return new();
    }
}
