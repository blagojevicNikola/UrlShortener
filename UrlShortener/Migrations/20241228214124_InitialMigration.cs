using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pair",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Invalidated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LongUrl = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ShortenUrl = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pair", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AccessTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Invalidated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IpAddress = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Referrer = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usage_pair_Id",
                        column: x => x.Id,
                        principalTable: "pair",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usage");

            migrationBuilder.DropTable(
                name: "pair");
        }
    }
}
