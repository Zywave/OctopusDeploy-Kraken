using Microsoft.Data.Entity;

namespace Kraken.Models
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<ReleaseBatchItem> ReleaseBatchItems { get; set; }
        public virtual DbSet<ReleaseBatch> ReleaseBatches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("kraken");

            modelBuilder.Entity<ReleaseBatchItem>().HasOne(e => e.Batch).WithMany(e => e.Items);
        }
    }
}
