namespace Inventory.Infrastructure.Persistence.Repositories
{
    using Inventory.Application.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Threading.Tasks;

    public class InventoryRepository : IInventoryRepository
    {
        private readonly InventoryDbContext _dbContext;
        public InventoryRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Domain.Entities.InventoryItem?> GetByProductIdAsync(Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.InventoryItems
                .FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);
        }
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
