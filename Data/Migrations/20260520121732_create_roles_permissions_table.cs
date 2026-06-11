using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Invoice_system.Migrations
{
    /// <inheritdoc />
    public partial class create_roles_permissions_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles_permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    dashboard = table.Column<bool>(type: "bit", nullable: false),
                    customers = table.Column<bool>(type: "bit", nullable: false),
                    products = table.Column<bool>(type: "bit", nullable: false),
                    sales = table.Column<bool>(type: "bit", nullable: false),
                    employees = table.Column<bool>(type: "bit", nullable: false),
                    Reports = table.Column<bool>(type: "bit", nullable: false),
                    settings = table.Column<bool>(type: "bit", nullable: false),
                    inventory = table.Column<bool>(type: "bit", nullable: false),
                    recovery = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles_permissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_roles_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_roles_permissions_role_id",
                table: "roles_permissions",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "roles_permissions");
        }
    }
}
