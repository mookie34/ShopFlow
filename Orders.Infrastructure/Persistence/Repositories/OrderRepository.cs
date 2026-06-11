namespace Orders.Infrastructure.Persistence.Repositories
{
    using Orders.Application.Interfaces;
    using Orders.Domain.Entities;
    using System.Threading.Tasks;

    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersDbContext _dbContext;
        public OrderRepository(OrdersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Domain.Entities.Order order, CancellationToken cancellationToken = default)
        {
            await _dbContext.Orders.AddAsync(order, cancellationToken);
        }
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Order> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
        {
            var order = await _dbContext.Orders.FindAsync(new object[] { orderId }, cancellationToken);
            if (order == null)
                return null;

            return order;
        }
    }
}
