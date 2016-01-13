using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Kraken.Migrations
{
    public partial class cleanup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_ReleaseBatchItem_ReleaseBatch_ReleaseBatchId", schema: "kraken", table: "ReleaseBatchItem");
            migrationBuilder.DropColumn(name: "LatestDeploymentId", schema: "kraken", table: "ReleaseBatchItem");
            migrationBuilder.DropColumn(name: "LatestReleaseId", schema: "kraken", table: "ReleaseBatchItem");
            migrationBuilder.DropColumn(name: "LatestTaskId", schema: "kraken", table: "ReleaseBatchItem");
            migrationBuilder.DropColumn(name: "NugetPackageId", schema: "kraken", table: "ReleaseBatchItem");
            migrationBuilder.AddColumn<string>(
                name: "ReleaseId",
                schema: "kraken",
                table: "ReleaseBatchItem",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseBatchItem_ReleaseBatch_ReleaseBatchId",
                schema: "kraken",
                table: "ReleaseBatchItem",
                column: "ReleaseBatchId",
                principalSchema: "kraken",
                principalTable: "ReleaseBatch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_ReleaseBatchItem_ReleaseBatch_ReleaseBatchId", schema: "kraken", table: "ReleaseBatchItem");
            migrationBuilder.DropColumn(name: "ReleaseId", schema: "kraken", table: "ReleaseBatchItem");
            migrationBuilder.AddColumn<string>(
                name: "LatestDeploymentId",
                schema: "kraken",
                table: "ReleaseBatchItem",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "LatestReleaseId",
                schema: "kraken",
                table: "ReleaseBatchItem",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "LatestTaskId",
                schema: "kraken",
                table: "ReleaseBatchItem",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "NugetPackageId",
                schema: "kraken",
                table: "ReleaseBatchItem",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseBatchItem_ReleaseBatch_ReleaseBatchId",
                schema: "kraken",
                table: "ReleaseBatchItem",
                column: "ReleaseBatchId",
                principalSchema: "kraken",
                principalTable: "ReleaseBatch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
