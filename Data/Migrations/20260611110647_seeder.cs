using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Invoice_system.Migrations
{
    /// <inheritdoc />
    public partial class seeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "role_title" },
                values: new object[,]
                {
                    { 1, "SuperAdmin" },
                    { 2, "Admin" }  
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "email", "password", "role_id", "status", "username" },
                values: new object[,]
                {
                    { 1, "superadmin@gmail.com", "superadmin", 1, 1, "superadmin" },
                    { 2, "admin@gmail.com",      "admin123",   2, 1, "admin"      }
                });

            migrationBuilder.InsertData(
                table: "roles_permissions",
                columns: new[] { "id", "customers", "dashboard", "employees", "inventory", "products", "Reports", "role_id", "sales", "settings", "recovery" },
                values: new object[] { 2, true, true, true, true, true, true, 2, true, true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles_permissions",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
