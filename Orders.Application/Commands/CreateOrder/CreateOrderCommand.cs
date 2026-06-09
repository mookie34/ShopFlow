using MediatR;
using Orders.Application.Contracts;

namespace Orders.Application.Commands.CreateOrder;

public record CreateOrderCommand(
    Guid CustomerId,
    Guid ProductId,
    int Quantity,
    decimal TotalAmount
) : IRequest<CreateOrderResponse>;