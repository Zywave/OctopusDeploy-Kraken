using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Kraken.Models;

namespace Kraken.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160118200516_auditinfo")]
    partial class auditinfo
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

                    b.Property<string>("DeployUserName")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 100);

                    b.Property<DateTimeOffset?>("SyncDateTime");

                    b.Property<string>("SyncEnvironmentId")
                        .HasAnnotation("MaxLength", 20);

                    b.Property<string>("SyncUserName")
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

                    b.Property<int>("ReleaseBatchId");

                    b.Property<string>("ReleaseId")
                        .HasAnnotation("MaxLength", 20);

                    b.Property<string>("ReleaseVersion")
                        .HasAnnotation("MaxLength", 20);

                    b.HasKey("Id");

                    b.HasAlternateKey("ReleaseBatchId", "ProjectId");
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
