using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailio.backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class create_user_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "email", "password", "role_id", "status", "username" },
                values: new object[] { 1, "superadmin@gmail.com", "186cf774c97b60a1c106ef718d10970a6a06e06bef89553d9ae65d938a886eae", 0, 1, "superadmin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
