using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailio.backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class create_categories_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    business_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                    table.ForeignKey(
                        name: "FK_categories_users_business_id",
                        column: x => x.business_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_categories_business_id",
                table: "categories",
                column: "business_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
