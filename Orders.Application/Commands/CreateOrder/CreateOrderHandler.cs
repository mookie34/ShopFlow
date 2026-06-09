namespace Orders.Application.Commands.CreateOrder
{
    using MediatR;
    using Orders.Application.Contracts;
    using Orders.Application.Interfaces;
    using Orders.Domain.Entities;
    using Shared.Events;
    using System.Text.Json;

    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOutboxRepository _outboxRepository;

        public CreateOrderHandler(IOrderRepository orderRepository, IOutboxRepository outboxRepository)
        {
            _orderRepository = orderRepository;
            _outboxRepository = outboxRepository;
        }

        public async Task<CreateOrderResponse> Handle(
            CreateOrderCommand command,
            CancellationToken cancellationToken)
        {
            //1. crea la entidad de dominio
            var order = Order.Create(
                command.CustomerId,
                command.ProductId,
                command.Quantity,
                command.TotalAmount);

            //2. Persiste la orden
            await _orderRepository.AddAsync(order, cancellationToken);

            //3. Guardar el evento en la tabla de Outbox
            var orderCreatedEvent = new OrderCreatedEvent
            {
                EventId = Guid.NewGuid(),
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt
            };

            await _outboxRepository.AddAsync(
                nameof(orderCreatedEvent),
                JsonSerializer.Serialize(orderCreatedEvent),
                cancellationToken
                );

            //4. Guardar todo en una sola transacción
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return new CreateOrderResponse
            (
                order.Id,
                order.Status.ToString(),
                order.CreatedAt
                );
        }
    }
}
