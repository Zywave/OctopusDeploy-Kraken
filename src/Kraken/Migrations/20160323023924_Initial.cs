using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace Kraken.Migrations
{
    public partial class Initial : Migration
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
                    DisplayName = table.Column<string>(nullable: true),
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
                    DeployDateTime = table.Column<DateTimeOffset>(nullable: true),
                    DeployEnvironmentId = table.Column<string>(nullable: true),
                    DeployEnvironmentName = table.Column<string>(nullable: true),
                    DeployUserName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsLocked = table.Column<bool>(nullable: false),
                    LockComment = table.Column<string>(nullable: true),
                    LockUserName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    SyncDateTime = table.Column<DateTimeOffset>(nullable: true),
                    SyncEnvironmentId = table.Column<string>(nullable: true),
                    SyncEnvironmentName = table.Column<string>(nullable: true),
                    SyncUserName = table.Column<string>(nullable: true),
                    UpdateDateTime = table.Column<DateTimeOffset>(nullable: true),
                    UpdateUserName = table.Column<string>(nullable: true)
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
                    ProjectSlug = table.Column<string>(nullable: false),
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
            migrationBuilder.CreateTable(
                name: "ReleaseBatchLogo",
                schema: "kraken",
                columns: table => new
                {
                    ReleaseBatchId = table.Column<int>(nullable: false),
                    Content = table.Column<byte[]>(nullable: true),
                    ContentType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseBatchLogo", x => x.ReleaseBatchId);
                    table.ForeignKey(
                        name: "FK_ReleaseBatchLogo_ReleaseBatch_ReleaseBatchId",
                        column: x => x.ReleaseBatchId,
                        principalSchema: "kraken",
                        principalTable: "ReleaseBatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateIndex(
                name: "IX_ReleaseBatch_Name",
                schema: "kraken",
                table: "ReleaseBatch",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ApplicationUser", schema: "kraken");
            migrationBuilder.DropTable(name: "ReleaseBatchItem", schema: "kraken");
            migrationBuilder.DropTable(name: "ReleaseBatchLogo", schema: "kraken");
            migrationBuilder.DropTable(name: "ReleaseBatch", schema: "kraken");
        }
    }
}
