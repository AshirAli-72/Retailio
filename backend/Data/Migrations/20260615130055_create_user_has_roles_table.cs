using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailio.backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class create_user_has_roles_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "employee_has_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    emp_id = table.Column<int>(type: "int", nullable: false),
                    roles_has_permission_id = table.Column<int>(type: "int", nullable: false),
                    business_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_has_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_employee_has_roles_businesses_business_id",
                        column: x => x.business_id,
                        principalTable: "businesses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_employee_has_roles_roles_has_permissions_roles_has_permission_id",
                        column: x => x.roles_has_permission_id,
                        principalTable: "roles_has_permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_employee_has_roles_business_id",
                table: "employee_has_roles",
                column: "business_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_has_roles_roles_has_permission_id",
                table: "employee_has_roles",
                column: "roles_has_permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_has_roles_emp_id",
                table: "employee_has_roles",
                column: "emp_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employee_has_roles");
        }
    }
}
