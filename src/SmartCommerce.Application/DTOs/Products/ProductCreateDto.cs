using SmartCommerce.Application.Common.Validation;

namespace SmartCommerce.Application.DTOs.Products;

public sealed record ProductCreateDto(
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    Guid CategoryId
)
{
    public Dictionary<string, string[]> Validate()
    {
        var errors = new Dictionary<string, string[]>();

        void Add(string field, string message)
        {
            if (errors.TryGetValue(field, out var existing))
                errors[field] = existing.Concat(new[] { message }).ToArray();
            else
                errors[field] = new[] { message };
        }

        if (string.IsNullOrWhiteSpace(Name))
            Add(nameof(Name), "Name is required.");
        else
        {
            var trimmed = Name.Trim();
            if (trimmed.Length < 2) Add(nameof(Name), "Name must be at least 2 characters.");
            if (trimmed.Length > 100) Add(nameof(Name), "Name must be at most 100 characters.");
        }

        if (Description is not null && Description.Length > 500)
            Add(nameof(Description), "Description must be at most 500 characters.");

        if (Price <= 0)
            Add(nameof(Price), "Price must be greater than 0.");

        if (Stock < 0)
            Add(nameof(Stock), "Stock cannot be negative.");

        if (CategoryId == Guid.Empty)
            Add(nameof(CategoryId), "CategoryId is required.");

        return errors;
    }
}
