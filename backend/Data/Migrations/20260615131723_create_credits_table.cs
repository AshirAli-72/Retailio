using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retailio.backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class create_credits_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "credits",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    total_credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    paid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    remaining = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    payment_method = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<int>(type: "int", nullable: true),
                    file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    business_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credits", x => x.id);
                    table.ForeignKey(
                        name: "FK_credits_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_credits_users_business_id",
                        column: x => x.business_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_credits_business_id",
                table: "credits",
                column: "business_id");

            migrationBuilder.CreateIndex(
                name: "IX_credits_customer_id",
                table: "credits",
                column: "customer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "credits");
        }
    }
}
