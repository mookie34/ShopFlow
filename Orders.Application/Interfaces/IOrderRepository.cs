namespace Orders.Application.Interfaces
{
    using Orders.Domain.Entities;
    using System.Threading.Tasks;

    public interface IOrderRepository
    {
        Task AddAsync(Order order, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<Order> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);
    }
}
