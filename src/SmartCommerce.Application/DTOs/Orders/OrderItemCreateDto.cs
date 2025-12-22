namespace SmartCommerce.Application.DTOs.Orders;

public sealed record OrderItemCreateDto(
    Guid ProductId,
    int Quantity
);
