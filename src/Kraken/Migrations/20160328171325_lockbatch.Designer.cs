using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Kraken.Models;

namespace Kraken.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160328171325_lockbatch")]
    partial class lockbatch
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("Relational:DefaultSchema", "kraken")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Kraken.Models.ApplicationUser", b =>
                {
                    b.Property<string>("UserName");

                    b.Property<string>("DisplayName");

                    b.Property<string>("OctopusApiKey");

                    b.HasKey("UserName");
                });

            modelBuilder.Entity("Kraken.Models.ReleaseBatch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset?>("DeployDateTime");

                    b.Property<string>("DeployEnvironmentId")
                        .HasAnnotation("MaxLength", 20);

                    b.Property<string>("DeployEnvironmentName")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("DeployUserName")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Description")
                        .HasAnnotation("MaxLength", 250);

                    b.Property<bool>("IsLocked");

                    b.Property<string>("LockComment")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<string>("LockUserName")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 100);

                    b.Property<DateTimeOffset?>("SyncDateTime");

                    b.Property<string>("SyncEnvironmentId")
                        .HasAnnotation("MaxLength", 20);

                    b.Property<string>("SyncEnvironmentName")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("SyncUserName")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<DateTimeOffset?>("UpdateDateTime");

                    b.Property<string>("UpdateUserName")
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();
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

                    b.Property<string>("ProjectSlug")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int>("ReleaseBatchId");

                    b.Property<string>("ReleaseId")
                        .HasAnnotation("MaxLength", 20);

                    b.Property<string>("ReleaseVersion")
                        .HasAnnotation("MaxLength", 20);

                    b.HasKey("Id");

                    b.HasAlternateKey("ReleaseBatchId", "ProjectId");
                });

            modelBuilder.Entity("Kraken.Models.ReleaseBatchLogo", b =>
                {
                    b.Property<int>("ReleaseBatchId");

                    b.Property<byte[]>("Content");

                    b.Property<string>("ContentType");

                    b.HasKey("ReleaseBatchId");
                });

            modelBuilder.Entity("Kraken.Models.ReleaseBatchItem", b =>
                {
                    b.HasOne("Kraken.Models.ReleaseBatch")
                        .WithMany()
                        .HasForeignKey("ReleaseBatchId");
                });

            modelBuilder.Entity("Kraken.Models.ReleaseBatchLogo", b =>
                {
                    b.HasOne("Kraken.Models.ReleaseBatch")
                        .WithOne()
                        .HasForeignKey("Kraken.Models.ReleaseBatchLogo", "ReleaseBatchId");
                });
        }
    }
}
