using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Kraken.Migrations
{
    public partial class batchitems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_ProjectBatchItem_ProjectBatch_ProjectBatchId", schema: "kraken", table: "ProjectBatchItem");
            migrationBuilder.AddForeignKey(
                name: "FK_ProjectBatchItem_ProjectBatch_ProjectBatchId",
                schema: "kraken",
                table: "ProjectBatchItem",
                column: "ProjectBatchId",
                principalSchema: "kraken",
                principalTable: "ProjectBatch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_ProjectBatchItem_ProjectBatch_ProjectBatchId", schema: "kraken", table: "ProjectBatchItem");
            migrationBuilder.AddForeignKey(
                name: "FK_ProjectBatchItem_ProjectBatch_ProjectBatchId",
                schema: "kraken",
                table: "ProjectBatchItem",
                column: "ProjectBatchId",
                principalSchema: "kraken",
                principalTable: "ProjectBatch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
