namespace Orders.Application.Interfaces
{
    public interface IInventoryClient
    {
        Task<bool> CheckStockAsync(Guid productId, int quantity, CancellationToken cancellationToken);
    }
}
