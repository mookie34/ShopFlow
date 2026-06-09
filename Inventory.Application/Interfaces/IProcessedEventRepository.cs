namespace Inventory.Application.Interfaces;

public interface IProcessedEventRepository
{
    Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task AddAsync(Guid eventId, CancellationToken cancellationToken = default);
}