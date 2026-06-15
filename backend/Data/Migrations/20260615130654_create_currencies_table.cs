using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailio.backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class create_currencies_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "currencies",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    symbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    exchange_rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    business_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_currencies", x => x.id);
                    table.ForeignKey(
                        name: "FK_currencies_users_business_id",
                        column: x => x.business_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_currencies_business_id",
                table: "currencies",
                column: "business_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "currencies");
        }
    }
}
