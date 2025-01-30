using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("select current_user;");
            migrationBuilder.CreateTable(
                name: "counter",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    max_value = table.Column<long>(type: "bigint", nullable: false),
                    current_starting_value = table.Column<long>(type: "bigint", nullable: false),
                    increment_value = table.Column<long>(type: "bigint", nullable: false),
                    concurrency = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    invalidated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_counter", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pair",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    invalidated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    long_url = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    shorten_url = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pair", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usage",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    access_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    pair_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invalidated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ip_address = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    referer = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usage", x => x.id);
                    table.ForeignKey(
                        name: "fk_usage_pair_pair_id",
                        column: x => x.pair_id,
                        principalTable: "pair",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_usage_pair_id",
                table: "usage",
                column: "pair_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "counter");

            migrationBuilder.DropTable(
                name: "usage");

            migrationBuilder.DropTable(
                name: "pair");
        }
    }
}
