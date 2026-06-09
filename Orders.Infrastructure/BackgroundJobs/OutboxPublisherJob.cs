namespace Orders.Infrastructure.BackgroundJobs
{
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Orders.Infrastructure.Persistence;
    using Shared.Events;
    using System.Linq;
    using System.Threading.Tasks;

    public class OutboxPublisherJob
    {
        private readonly OrdersDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public OutboxPublisherJob(OrdersDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
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
                if (message.EventType == nameof(OrderCreatedEvent))
                {
                    var orderCreatedEvent = System.Text.Json.JsonSerializer.Deserialize<OrderCreatedEvent>(message.Payload);
                    await _publishEndpoint.Publish(orderCreatedEvent);
                }

                message.MarkAsPublished();
            }
            await _context.SaveChangesAsync();
        }
    }
}
