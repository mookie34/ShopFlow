using Inventory.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    public class ProcessedEventRepository : IProcessedEventRepository
    {
        private readonly InventoryDbContext _dbContext;
        public ProcessedEventRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProcessedEvents.AnyAsync(e => e.EventId == eventId, cancellationToken);
        }

        public async Task AddAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var processedEvent = ProcessedEvent.Create(eventId);
            await _dbContext.ProcessedEvents.AddAsync(processedEvent, cancellationToken);
        }

        public async Task SaveChangeAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
