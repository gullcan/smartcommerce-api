namespace SmartCommerce.Application.DTOs.Orders;

public sealed record OrderItemResponseDto(
    Guid Id,
    Guid ProductId,
    string? ProductName,
    int Quantity,
    decimal UnitPrice
);
