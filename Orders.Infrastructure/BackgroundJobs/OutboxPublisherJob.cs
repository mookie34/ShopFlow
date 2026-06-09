using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Infrastructure.Persistence;
using Shared.Events;
using System.Text.Json;

namespace Orders.Infrastructure.BackgroundJobs;

public class OutboxPublisherJob
{
    private readonly OrdersDbContext _context;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public OutboxPublisherJob(
        OrdersDbContext context,
        ISendEndpointProvider sendEndpointProvider)
    {
        _context = context;
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task ExecuteAsync()
    {
        var pendingMessages = await _context.OutboxMessages
            .Where(m => !m.Published)
            .OrderBy(m => m.CreatedAt)
            .Take(20)
            .ToListAsync();

        foreach (var message in pendingMessages)
        {
            if (message.EventType.Equals(nameof(OrderCreatedEvent), StringComparison.OrdinalIgnoreCase))
            {
                var orderCreatedEvent = JsonSerializer
                    .Deserialize<OrderCreatedEvent>(message.Payload);

                if (orderCreatedEvent is not null)
                {
                    var endpoint = await _sendEndpointProvider
                        .GetSendEndpoint(new Uri("queue:order-created"));

                    await endpoint.Send(orderCreatedEvent);
                }
            }

            message.MarkAsPublished();
        }

        await _context.SaveChangesAsync();
    }
}