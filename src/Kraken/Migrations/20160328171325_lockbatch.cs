using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Kraken.Migrations
{
    public partial class lockbatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_ReleaseBatchItem_ReleaseBatch_ReleaseBatchId", schema: "kraken", table: "ReleaseBatchItem");
            migrationBuilder.DropForeignKey(name: "FK_ReleaseBatchLogo_ReleaseBatch_ReleaseBatchId", schema: "kraken", table: "ReleaseBatchLogo");
            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                schema: "kraken",
                table: "ReleaseBatch",
                nullable: false,
                defaultValue: false);
            migrationBuilder.AddColumn<string>(
                name: "LockComment",
                schema: "kraken",
                table: "ReleaseBatch",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "LockUserName",
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
            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseBatchLogo_ReleaseBatch_ReleaseBatchId",
                schema: "kraken",
                table: "ReleaseBatchLogo",
                column: "ReleaseBatchId",
                principalSchema: "kraken",
                principalTable: "ReleaseBatch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_ReleaseBatchItem_ReleaseBatch_ReleaseBatchId", schema: "kraken", table: "ReleaseBatchItem");
            migrationBuilder.DropForeignKey(name: "FK_ReleaseBatchLogo_ReleaseBatch_ReleaseBatchId", schema: "kraken", table: "ReleaseBatchLogo");
            migrationBuilder.DropColumn(name: "IsLocked", schema: "kraken", table: "ReleaseBatch");
            migrationBuilder.DropColumn(name: "LockComment", schema: "kraken", table: "ReleaseBatch");
            migrationBuilder.DropColumn(name: "LockUserName", schema: "kraken", table: "ReleaseBatch");
            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseBatchItem_ReleaseBatch_ReleaseBatchId",
                schema: "kraken",
                table: "ReleaseBatchItem",
                column: "ReleaseBatchId",
                principalSchema: "kraken",
                principalTable: "ReleaseBatch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseBatchLogo_ReleaseBatch_ReleaseBatchId",
                schema: "kraken",
                table: "ReleaseBatchLogo",
                column: "ReleaseBatchId",
                principalSchema: "kraken",
                principalTable: "ReleaseBatch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
