using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

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
                name: "ReleaseBatch",
                schema: "kraken",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    SyncDateTime = table.Column<DateTimeOffset>(nullable: true),
                    SyncEnvironmentId = table.Column<string>(nullable: true),
                    SyncUserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseBatch", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "ReleaseBatchItem",
                schema: "kraken",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<string>(nullable: false),
                    ProjectName = table.Column<string>(nullable: false),
                    ReleaseBatchId = table.Column<int>(nullable: false),
                    ReleaseId = table.Column<string>(nullable: true),
                    ReleaseVersion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseBatchItem", x => x.Id);
                    table.UniqueConstraint("AK_ReleaseBatchItem_ReleaseBatchId_ProjectId", x => new { x.ReleaseBatchId, x.ProjectId });
                    table.ForeignKey(
                        name: "FK_ReleaseBatchItem_ReleaseBatch_ReleaseBatchId",
                        column: x => x.ReleaseBatchId,
                        principalSchema: "kraken",
                        principalTable: "ReleaseBatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ApplicationUser", schema: "kraken");
            migrationBuilder.DropTable(name: "ReleaseBatchItem", schema: "kraken");
            migrationBuilder.DropTable(name: "ReleaseBatch", schema: "kraken");
        }
    }
}
