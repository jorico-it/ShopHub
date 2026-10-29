using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopHub.Web.Migrations
{
    /// <inheritdoc />
    public partial class MandantIbanMitLeerzeichen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Iban",
                table: "jorMandanten",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(34)",
                oldMaxLength: 34,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Iban",
                table: "jorMandanten",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(45)",
                oldMaxLength: 45,
                oldNullable: true);
        }
    }
}
