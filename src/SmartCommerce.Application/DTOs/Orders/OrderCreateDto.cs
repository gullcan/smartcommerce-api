using SmartCommerce.Application.Common.Validation;

namespace SmartCommerce.Application.DTOs.Orders;

public sealed record OrderCreateItemDto(string ProductId, int Quantity);

public sealed record OrderCreateDto(List<OrderCreateItemDto> Items)
{
    public Dictionary<string, string[]> Validate()
    {
        if (Items is null || Items.Count == 0)
            return ValidationErrors.Single(nameof(Items), "At least 1 item is required.");

        if (Items.Count > 50)
            return ValidationErrors.Single(nameof(Items), "Too many items (max 50).");

        for (var i = 0; i < Items.Count; i++)
        {
            var it = Items[i];

            if (!Guid.TryParse(it.ProductId, out _))
                return ValidationErrors.Single(nameof(Items), $"Items[{i}].ProductId is invalid.");

            if (it.Quantity <= 0)
                return ValidationErrors.Single(nameof(Items), $"Items[{i}].Quantity must be > 0.");
        }

        return new();
    }
}
