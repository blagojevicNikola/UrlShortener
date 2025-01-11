using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Migrations
{
    /// <inheritdoc />
    public partial class Shadowforeignkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_usage_pair_Id",
                table: "usage");

            migrationBuilder.AddColumn<Guid>(
                name: "PairId",
                table: "usage",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_usage_PairId",
                table: "usage",
                column: "PairId");

            migrationBuilder.AddForeignKey(
                name: "FK_usage_pair_PairId",
                table: "usage",
                column: "PairId",
                principalTable: "pair",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_usage_pair_PairId",
                table: "usage");

            migrationBuilder.DropIndex(
                name: "IX_usage_PairId",
                table: "usage");

            migrationBuilder.DropColumn(
                name: "PairId",
                table: "usage");

            migrationBuilder.AddForeignKey(
                name: "FK_usage_pair_Id",
                table: "usage",
                column: "Id",
                principalTable: "pair",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
