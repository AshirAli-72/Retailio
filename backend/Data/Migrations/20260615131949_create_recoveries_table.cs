using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailio.backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class create_recoveries_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "recoveries",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    credit_id = table.Column<int>(type: "int", nullable: true),
                    date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    total_credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    paid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    remaining = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: true),
                    file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    business_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recoveries", x => x.id);
                    table.ForeignKey(
                        name: "FK_recoveries_users_business_id",
                        column: x => x.business_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_recoveries_business_id",
                table: "recoveries",
                column: "business_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "recoveries");
        }
    }
}
