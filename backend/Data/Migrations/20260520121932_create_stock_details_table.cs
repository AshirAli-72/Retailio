using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailio.Migrations
{
    /// <inheritdoc />
    public partial class create_stock_details_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stock_details",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    item_barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    pur_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    sale_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    whole_sale_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    stock_alert = table.Column<int>(type: "int", nullable: false),
                    date_of_manafacture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_of_expiry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    total_pur_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_details", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "stock_details");
        }
    }
}
