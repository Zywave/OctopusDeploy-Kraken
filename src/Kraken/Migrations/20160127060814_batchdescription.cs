using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Kraken.Migrations
{
    public partial class batchdescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_ReleaseBatchItem_ReleaseBatch_ReleaseBatchId", schema: "kraken", table: "ReleaseBatchItem");
            migrationBuilder.AddColumn<string>(
                name: "Description",
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
            migrationBuilder.DropColumn(name: "Description", schema: "kraken", table: "ReleaseBatch");
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
