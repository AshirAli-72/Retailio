using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Retailio.backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class remove_default_permissions_seed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: 9);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "names", "slugs", "business_id" },
                values: new object[,]
                {
                    { 1, "Dashboard", "dashboard", null },
                    { 2, "Customers", "customers", null },
                    { 3, "Products", "products", null },
                    { 4, "Sales", "sales", null },
                    { 5, "Settings", "settings", null },
                    { 6, "Inventory", "inventory", null },
                    { 7, "Employees", "employees", null },
                    { 8, "Reports", "reports", null },
                    { 9, "Recovery", "recovery", null }
                });
        }
    }
}
