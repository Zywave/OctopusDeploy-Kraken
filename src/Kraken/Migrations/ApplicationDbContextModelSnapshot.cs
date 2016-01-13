using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Kraken.Models;

namespace Kraken.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("Relational:DefaultSchema", "kraken")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Kraken.Models.ApplicationUser", b =>
                {
                    b.Property<string>("UserName");

                    b.Property<string>("OctopusApiKey");

                    b.HasKey("UserName");
                });

            modelBuilder.Entity("Kraken.Models.ReleaseBatch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Kraken.Models.ReleaseBatchItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 20);

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("ProjectVersion")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 20);

                    b.Property<int>("ReleaseBatchId");

                    b.Property<string>("ReleaseId")
                        .HasAnnotation("MaxLength", 20);

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Kraken.Models.ReleaseBatchItem", b =>
                {
                    b.HasOne("Kraken.Models.ReleaseBatch")
                        .WithMany()
                        .HasForeignKey("ReleaseBatchId");
                });
        }
    }
}
