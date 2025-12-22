namespace SmartCommerce.Application.DTOs.Orders;

public sealed record OrderCreateDto(
    Guid UserId,
    IReadOnlyList<OrderItemCreateDto> Items
);
