using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace Kraken.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema("kraken");
            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                schema: "kraken",
                columns: table => new
                {
                    UserName = table.Column<string>(nullable: false),
                    OctopusApiKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.UserName);
                });
            migrationBuilder.CreateTable(
                name: "ProjectBatch",
                schema: "kraken",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectBatch", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "ProjectBatchItem",
                schema: "kraken",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LatestDeploymentId = table.Column<string>(nullable: true),
                    LatestReleaseId = table.Column<string>(nullable: true),
                    LatestTaskId = table.Column<string>(nullable: true),
                    NugetPackageId = table.Column<string>(nullable: true),
                    ProjectBatchId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<string>(nullable: false),
                    ProjectName = table.Column<string>(nullable: false),
                    ProjectVersion = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectBatchItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectBatchItem_ProjectBatch_ProjectBatchId",
                        column: x => x.ProjectBatchId,
                        principalSchema: "kraken",
                        principalTable: "ProjectBatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ApplicationUser", schema: "kraken");
            migrationBuilder.DropTable(name: "ProjectBatchItem", schema: "kraken");
            migrationBuilder.DropTable(name: "ProjectBatch", schema: "kraken");
        }
    }
}
