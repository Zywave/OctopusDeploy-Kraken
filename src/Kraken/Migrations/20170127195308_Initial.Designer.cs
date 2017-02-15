using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Kraken.Models;

namespace Kraken.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20170127195308_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultSchema("kraken")
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Kraken.Models.ApplicationUser", b =>
                {
                    b.Property<string>("UserName")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("DisplayName");

                    b.Property<string>("OctopusApiKey");

                    b.HasKey("UserName");

                    b.ToTable("ApplicationUser");
                });

            modelBuilder.Entity("Kraken.Models.ReleaseBatch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset?>("DeployDateTime");

                    b.Property<string>("DeployEnvironmentId")
                        .HasMaxLength(20);

                    b.Property<string>("DeployEnvironmentName")
                        .HasMaxLength(50);

                    b.Property<string>("DeployUserName")
                        .HasMaxLength(50);

                    b.Property<string>("Description")
                        .HasMaxLength(250);

                    b.Property<bool>("IsLocked");

                    b.Property<string>("LockComment")
                        .HasMaxLength(100);

                    b.Property<string>("LockUserName")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTimeOffset?>("SyncDateTime");

                    b.Property<string>("SyncEnvironmentId")
                        .HasMaxLength(20);

                    b.Property<string>("SyncEnvironmentName")
                        .HasMaxLength(50);

                    b.Property<string>("SyncUserName")
                        .HasMaxLength(50);

                    b.Property<DateTimeOffset?>("UpdateDateTime");

                    b.Property<string>("UpdateUserName")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ReleaseBatch");
                });

            modelBuilder.Entity("Kraken.Models.ReleaseBatchItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("ProjectSlug")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("ReleaseBatchId");

                    b.Property<string>("ReleaseId")
                        .HasMaxLength(20);

                    b.Property<string>("ReleaseVersion")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.HasAlternateKey("ReleaseBatchId", "ProjectId");

                    b.ToTable("ReleaseBatchItem");
                });

            modelBuilder.Entity("Kraken.Models.ReleaseBatchLogo", b =>
                {
                    b.Property<int>("ReleaseBatchId");

                    b.Property<byte[]>("Content");

                    b.Property<string>("ContentType");

                    b.HasKey("ReleaseBatchId");

                    b.ToTable("ReleaseBatchLogo");
                });

            modelBuilder.Entity("Kraken.Models.ReleaseBatchItem", b =>
                {
                    b.HasOne("Kraken.Models.ReleaseBatch", "Batch")
                        .WithMany("Items")
                        .HasForeignKey("ReleaseBatchId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Kraken.Models.ReleaseBatchLogo", b =>
                {
                    b.HasOne("Kraken.Models.ReleaseBatch", "Batch")
                        .WithOne("Logo")
                        .HasForeignKey("Kraken.Models.ReleaseBatchLogo", "ReleaseBatchId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
