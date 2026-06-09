namespace Orders.Application.Interfaces
{
    using System.Threading.Tasks;

    public interface IOutboxRepository
    {
        Task AddAsync(string eventType, string payload, CancellationToken cancellationToken = default);
    }
}
