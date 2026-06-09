namespace Orders.Domain.Entities
{
    using Orders.Domain.Enums;
    using System;

    public class Order
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal TotalAmount { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Order() { }

        public static Order Create(Guid customerId, Guid productId, int quantity, decimal totalAmount)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            if (totalAmount <= 0)
                throw new ArgumentException("Total amount must be greater than zero.", nameof(totalAmount));

            return new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ProductId = productId,
                Quantity = quantity,
                TotalAmount = totalAmount,
                Status = OrderStatus.pending,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Confirm() => Status = OrderStatus.confirmed;
        public void Cancel() => Status = OrderStatus.canceled;
    }
}
