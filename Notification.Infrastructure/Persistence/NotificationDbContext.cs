using Microsoft.EntityFrameworkCore;

namespace Notification.Infrastructure.Persistence
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options) { }

        public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProcessedEvent>(entity =>
            {
                entity.HasKey(e => e.EventId);
                entity.Property(e => e.ProcessedAt).IsRequired();
            });
        }
    }
}
