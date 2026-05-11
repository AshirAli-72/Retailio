using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Invoice_system.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "employee",
                columns: new[] { "id", "address", "cnic", "date", "email", "emp_code", "full_name", "image_path", "mobile_no", "salary", "status" },
                values: new object[] { 1, "Admin Address", "00000-0000000-0", "1-1-2024", "admin@gmail.com", "EMP-001", "Admin", null, "0000-0000000", 0m, "Active" });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "role_title" },
                values: new object[] { 1, "Admin" });

            migrationBuilder.InsertData(
                table: "roles_permissions",
                columns: new[] { "id", "customer_report", "customers", "daily_summary", "dashboard", "employee_report", "employees", "inventory", "invoice_report", "invoices", "product_report", "products", "Reports", "returns_report", "role_id", "sale_report", "sales", "settings" },
                values: new object[] { 1, true, true, true, true, true, true, true, true, true, true, true, true, true, 1, true, true, true });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "email", "emp_id", "password", "role_id", "status", "username" },
                values: new object[] { 1, "admin@gmail.com", 1, "admin123", 1, "Active", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles_permissions",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "employee",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
