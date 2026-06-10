using Microsoft.EntityFrameworkCore;
using Notification.Application.Interfaces;

namespace Notification.Infrastructure.Persistence.Repositories
{
    public class ProcessedEventRepository : IProcessedEventRepository
    {
        private readonly NotificationDbContext _context;
        public ProcessedEventRepository(NotificationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await _context.ProcessedEvents.AnyAsync(e => e.EventId == eventId, cancellationToken);
        }
        public async Task AddAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var processedEvent = ProcessedEvent.Create(eventId);
            await _context.ProcessedEvents.AddAsync(processedEvent, cancellationToken);
        }
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}