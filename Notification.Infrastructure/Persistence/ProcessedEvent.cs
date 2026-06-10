namespace Notification.Infrastructure.Persistence
{
    using System;

    public class ProcessedEvent
    {
        public Guid EventId { get; private set; }
        public DateTime ProcessedAt { get; private set; }

        private ProcessedEvent() { }

        public static ProcessedEvent Create(Guid eventId)
        {
            return new ProcessedEvent
            {
                EventId = eventId,
                ProcessedAt = DateTime.UtcNow
            };
        }
    }
}
