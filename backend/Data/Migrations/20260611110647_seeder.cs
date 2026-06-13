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
            // Seed only the SuperAdmin role — all other roles are created dynamically
            // when users register or employees are added through the UI.
            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "role_title" },
                values: new object[] { 1, "SuperAdmin" });

            // Seed only the SuperAdmin user — all other users register through the UI.
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "email", "password", "role_id", "status", "username" },
                values: new object[] { 1, "superadmin@gmail.com", "186cf774c97b60a1c106ef718d10970a6a06e06bef89553d9ae65d938a886eae", 1, 1, "superadmin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
