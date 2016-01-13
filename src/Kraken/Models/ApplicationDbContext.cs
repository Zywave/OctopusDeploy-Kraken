using Microsoft.Data.Entity;

namespace Kraken.Models
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<ProjectBatchItem> ProjectBatchItems { get; set; }
        public virtual DbSet<ProjectBatch> ProjectBatches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("kraken");

            modelBuilder.Entity<ProjectBatchItem>().HasOne(e => e.Batch).WithMany(e => e.Items);
        }
    }
}
