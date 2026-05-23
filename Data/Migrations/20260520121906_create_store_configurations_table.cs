using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Invoice_system.Migrations
{
    /// <inheritdoc />
    public partial class create_store_configurations_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "store_configurations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    shop_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    owner_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    business_nature = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    branch = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    shop_no = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    phone_1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    phone_2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    logo_path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_configurations", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "store_configurations");
        }
    }
}
