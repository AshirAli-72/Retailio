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
                name: "user_has_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    roles_has_permission_id = table.Column<int>(type: "int", nullable: false),
                    business_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_has_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_has_roles_businesses_business_id",
                        column: x => x.business_id,
                        principalTable: "businesses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_has_roles_roles_has_permissions_roles_has_permission_id",
                        column: x => x.roles_has_permission_id,
                        principalTable: "roles_has_permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_has_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_has_roles_business_id",
                table: "user_has_roles",
                column: "business_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_has_roles_roles_has_permission_id",
                table: "user_has_roles",
                column: "roles_has_permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_has_roles_user_id",
                table: "user_has_roles",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_has_roles");
        }
    }
}
