namespace Shared.Events
{
    using System;

    public record OrderCreatedEvent
    {
        public Guid EventId { get; init; }
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public decimal TotalAmount { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
