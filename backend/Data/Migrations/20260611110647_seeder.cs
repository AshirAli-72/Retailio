using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailio.Migrations
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
                    { 1, "superadmin@gmail.com", "186cf774c97b60a1c106ef718d10970a6a06e06bef89553d9ae65d938a886eae", 1, 1, "superadmin" },
                    { 2, "admin@gmail.com",      "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9", 2, 1, "admin"      }
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
