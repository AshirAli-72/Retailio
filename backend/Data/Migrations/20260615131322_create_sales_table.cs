using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailio.backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class create_sales_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sales",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    inv_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customer_id = table.Column<int>(type: "int", nullable: true),
                    date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gross_total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    net_payable = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    paid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    due = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: true),
                    payment_method = table.Column<int>(type: "int", nullable: true),
                    business_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sales", x => x.id);
                    table.ForeignKey(
                        name: "FK_sales_users_business_id",
                        column: x => x.business_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_sales_business_id",
                table: "sales",
                column: "business_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sales");
        }
    }
}
