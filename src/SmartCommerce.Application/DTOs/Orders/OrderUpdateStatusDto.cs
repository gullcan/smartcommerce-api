using SmartCommerce.Application.Common.Validation;

namespace SmartCommerce.Application.DTOs.Orders;

public sealed record OrderUpdateStatusDto(string Status)
{
    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pending", "Paid", "Shipped", "Cancelled"
    };

    public Dictionary<string, string[]> Validate()
    {
        if (string.IsNullOrWhiteSpace(Status))
            return ValidationErrors.Single(nameof(Status), "Status is required.");

        if (!Allowed.Contains(Status.Trim()))
            return ValidationErrors.Single(nameof(Status), "Status must be one of: Pending, Paid, Shipped, Cancelled.");

        return new();
    }
}
