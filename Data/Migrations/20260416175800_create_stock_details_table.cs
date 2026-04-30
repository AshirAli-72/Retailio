using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Invoice_system.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260416175800_create_stock_details_table")]
    public partial class create_stock_details_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stock_details",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    item_barcode = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    pur_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    sale_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    whole_sale_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    stock_alert = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    date_of_manafacture = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_of_expiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    total_pur_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_details", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_stock_details_item_barcode",
                table: "stock_details",
                column: "item_barcode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stock_details");
        }
    }
}
