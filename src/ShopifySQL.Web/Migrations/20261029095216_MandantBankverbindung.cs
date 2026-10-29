using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopifySQL.Web.Migrations
{
    /// <inheritdoc />
    public partial class MandantBankverbindung : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bankname",
                table: "jorMandanten",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Iban",
                table: "jorMandanten",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bankname",
                table: "jorMandanten");

            migrationBuilder.DropColumn(
                name: "Iban",
                table: "jorMandanten");
        }
    }
}
