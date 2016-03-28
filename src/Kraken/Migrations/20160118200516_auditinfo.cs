using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Kraken.Migrations
{
    public partial class auditinfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_ReleaseBatchItem_ReleaseBatch_ReleaseBatchId", schema: "kraken", table: "ReleaseBatchItem");
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeployDateTime",
                schema: "kraken",
                table: "ReleaseBatch",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "DeployEnvironmentId",
                schema: "kraken",
                table: "ReleaseBatch",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "DeployUserName",
                schema: "kraken",
                table: "ReleaseBatch",
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
            migrationBuilder.DropColumn(name: "DeployDateTime", schema: "kraken", table: "ReleaseBatch");
            migrationBuilder.DropColumn(name: "DeployEnvironmentId", schema: "kraken", table: "ReleaseBatch");
            migrationBuilder.DropColumn(name: "DeployUserName", schema: "kraken", table: "ReleaseBatch");
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
