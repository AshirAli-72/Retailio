using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailio.backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class create_subscriptions_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    business_id = table.Column<int>(type: "int", nullable: false),
                    plan = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    started_at = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    expires_at = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_subscriptions_users_business_id",
                        column: x => x.business_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_business_id",
                table: "subscriptions",
                column: "business_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscriptions");
        }
    }
}
