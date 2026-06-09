namespace Orders.Infrastructure.Persistence.Repositories
{
    using Orders.Application.Interfaces;
    using System.Threading.Tasks;

    public class OutboxRepository : IOutboxRepository
    {
        private readonly OrdersDbContext _dbContext;
        public OutboxRepository(OrdersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(string eventType, string payload, CancellationToken cancellationToken = default)
        {
            var message = OutboxMessage.Create(eventType, payload);
            await _dbContext.OutboxMessages.AddAsync(message, cancellationToken);
        }
    }
}
