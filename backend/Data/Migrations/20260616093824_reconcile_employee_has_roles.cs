using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailio.backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class reconcile_employee_has_roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employee_has_roles_businesses_business_id",
                table: "employee_has_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_employee_has_roles_roles_has_permissions_roles_has_permission_id",
                table: "employee_has_roles");

            migrationBuilder.DropIndex(
                name: "IX_employee_has_roles_business_id",
                table: "employee_has_roles");

            migrationBuilder.DropIndex(
                name: "IX_employee_has_roles_emp_id",
                table: "employee_has_roles");

            migrationBuilder.DropIndex(
                name: "IX_employee_has_roles_roles_has_permission_id",
                table: "employee_has_roles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_employee_has_roles_business_id",
                table: "employee_has_roles",
                column: "business_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_has_roles_emp_id",
                table: "employee_has_roles",
                column: "emp_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_has_roles_roles_has_permission_id",
                table: "employee_has_roles",
                column: "roles_has_permission_id");

            migrationBuilder.AddForeignKey(
                name: "FK_employee_has_roles_businesses_business_id",
                table: "employee_has_roles",
                column: "business_id",
                principalTable: "businesses",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_employee_has_roles_employee_emp_id",
                table: "employee_has_roles",
                column: "emp_id",
                principalTable: "employee",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_employee_has_roles_roles_has_permissions_roles_has_permission_id",
                table: "employee_has_roles",
                column: "roles_has_permission_id",
                principalTable: "roles_has_permissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
