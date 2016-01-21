using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

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

            modelBuilder.Entity<ApplicationUser>().HasKey(e => e.UserName);

            modelBuilder.Entity<ReleaseBatch>().HasKey(e => e.Id);
            modelBuilder.Entity<ReleaseBatch>().HasIndex(e => e.Name).IsUnique();

            modelBuilder.Entity<ReleaseBatch>().HasKey(e => e.Id);
            modelBuilder.Entity<ReleaseBatchItem>().HasOne(e => e.Batch).WithMany(e => e.Items).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ReleaseBatchItem>().HasAlternateKey(e => new { e.ReleaseBatchId, e.ProjectId });
        }
    }
}
