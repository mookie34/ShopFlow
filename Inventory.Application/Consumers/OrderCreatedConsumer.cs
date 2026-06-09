using Inventory.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Events;

namespace Inventory.Application.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IProcessedEventRepository _processedEventRepository;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(
        IInventoryRepository inventoryRepository,
        IProcessedEventRepository processedEventRepository,
        ILogger<OrderCreatedConsumer> logger)
    {
        _inventoryRepository = inventoryRepository;
        _processedEventRepository = processedEventRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("Received OrderCreatedEvent: {EventId} for Product: {ProductId}",
            message.EventId, message.ProductId);

        // Idempotencia
        var alreadyProcessed = await _processedEventRepository
            .ExistsAsync(message.EventId, context.CancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogInformation("Event {EventId} already processed. Skipping.", message.EventId);
            return;
        }

        // Reducir stock
        var item = await _inventoryRepository
            .GetByProductIdAsync(message.ProductId, context.CancellationToken);

        if (item == null)
        {
            _logger.LogWarning("Product {ProductId} not found in inventory", message.ProductId);
            throw new InvalidOperationException(
                $"Product {message.ProductId} not found in inventory");
        }

        _logger.LogInformation("Current stock for product {ProductId}: {Stock}",
            message.ProductId, item.Stock);

        item.ReduceStock(message.Quantity);

        _logger.LogInformation("Stock reduced by {Quantity}. New stock: {Stock}",
            message.Quantity, item.Stock);

        await _processedEventRepository
            .AddAsync(message.EventId, context.CancellationToken);

        await _inventoryRepository.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("Event {EventId} processed successfully", message.EventId);
    }
}