using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopHub.Web.Migrations
{
    /// <inheritdoc />
    public partial class MarkeLogoEtikettenUndRegistrierungsnummer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "LogoEtiketten",
                table: "jorMarken",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoEtikettenContentType",
                table: "jorMarken",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Registrierungsnummer",
                table: "jorMarken",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoEtiketten",
                table: "jorMarken");

            migrationBuilder.DropColumn(
                name: "LogoEtikettenContentType",
                table: "jorMarken");

            migrationBuilder.DropColumn(
                name: "Registrierungsnummer",
                table: "jorMarken");
        }
    }
}
