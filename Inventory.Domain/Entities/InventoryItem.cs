namespace Inventory.Domain.Entities
{
    using System;

    public class InventoryItem
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public int Stock { get; private set; }

        private InventoryItem() { }
        public static InventoryItem Create(Guid productId, int initialStock)
        {
            if (initialStock < 0)
            {
                throw new ArgumentException("Initial stock cannot be negative.", nameof(initialStock));
            }

            return new InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Stock = initialStock
            };
        }

        public void ReduceStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            }
            if (Stock < quantity)
            {
                throw new InvalidOperationException("Insufficient stock to reduce.");
            }
            Stock -= quantity;
        }
    }
}
