namespace Orders.Infrastructure.Persistence
{
    using System;

    public class OutboxMessage
    {
        public Guid Id { get; private set; }
        public string EventType { get; private set; }
        public string Payload { get; private set; }
        public bool Published { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private OutboxMessage() { }
        public static OutboxMessage Create(string eventType, string payload)
        {
            return new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventType = eventType,
                Payload = payload,
                Published = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void MarkAsPublished() => Published = true;

    }
}
