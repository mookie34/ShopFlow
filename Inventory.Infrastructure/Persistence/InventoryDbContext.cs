namespace Inventory.Infrastructure.Persistence
{
    using Inventory.Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
        public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductId).IsRequired();
                entity.Property(e => e.Stock).IsRequired();
                entity.HasIndex(e => e.ProductId).IsUnique();
            });
            modelBuilder.Entity<ProcessedEvent>(entity =>
            {
                entity.HasKey(e => e.EventId);
                entity.Property(e => e.ProcessedAt).IsRequired();
            });
        }
    }
}
