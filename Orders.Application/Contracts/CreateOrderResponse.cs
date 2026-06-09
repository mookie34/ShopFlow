namespace Orders.Application.Contracts
{
    using System;

    public record CreateOrderResponse(
        Guid OrderId,
        string Status,
        DateTime CreatedAt
    );
}
