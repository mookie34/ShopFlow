using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.Interfaces;
using Shared.Events;

namespace Notification.Application.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly IProcessedEventRepository _processedEventRepository;
        private readonly ILogger<OrderCreatedConsumer> _logger;
        public OrderCreatedConsumer(IProcessedEventRepository processedEventRepository,
            ILogger<OrderCreatedConsumer> logger)
        {
            _processedEventRepository = processedEventRepository;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Received OrderCreatedEvent: EventId={EventId} for Order: OrderId={OrderId}, ",
                message.EventId, message.OrderId);

            //Idempotencia
            var alreadyProcessed = await _processedEventRepository.ExistsAsync(message.EventId, context.CancellationToken);

            if (alreadyProcessed)
            {
                _logger.LogInformation("Event with EventId={EventId} has already been processed. Skipping.", message.EventId);
                return;
            }

            //Simula envio de email
            _logger.LogInformation(
                "Sending email to customer {CustomerId}: Your order {OrderId} has been confirmed. Total: {Total}",
                message.CustomerId, message.OrderId, message.TotalAmount);

            //Registra o evento como processado
            await _processedEventRepository.AddAsync(message.EventId, context.CancellationToken);
            await _processedEventRepository.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("Notification sent for event {EventId}.", message.EventId);
        }
    }
}
