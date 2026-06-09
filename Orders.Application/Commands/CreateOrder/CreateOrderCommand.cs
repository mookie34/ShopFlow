namespace Orders.Application.Commands.CreateOrder
{
    using MediatR;
    using Orders.Application.Contracts;
    using System;

    public record CreateOrderCommand(
        Guid CustomerId,
        Guid ProductId,
        int Quantity,
        decimal TotalAmount
    ) : IRequest<CreateOrderResponse>;
}
