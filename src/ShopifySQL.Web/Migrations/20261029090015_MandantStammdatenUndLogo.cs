using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopifySQL.Web.Migrations
{
    /// <inheritdoc />
    public partial class MandantStammdatenUndLogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EMail",
                table: "jorMandanten",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EoriNummer",
                table: "jorMandanten",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HrbNummer",
                table: "jorMandanten",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Land",
                table: "jorMandanten",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Logo",
                table: "jorMandanten",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoContentType",
                table: "jorMandanten",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ort",
                table: "jorMandanten",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Plz",
                table: "jorMandanten",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Steuernummer",
                table: "jorMandanten",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Strasse",
                table: "jorMandanten",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefon",
                table: "jorMandanten",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UstIdNr",
                table: "jorMandanten",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EMail",
                table: "jorMandanten");

            migrationBuilder.DropColumn(
                name: "EoriNummer",
                table: "jorMandanten");

            migrationBuilder.DropColumn(
                name: "HrbNummer",
                table: "jorMandanten");

            migrationBuilder.DropColumn(
                name: "Land",
                table: "jorMandanten");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "jorMandanten");

            migrationBuilder.DropColumn(
                name: "LogoContentType",
                table: "jorMandanten");

            migrationBuilder.DropColumn(
                name: "Ort",
                table: "jorMandanten");

            migrationBuilder.DropColumn(
                name: "Plz",
                table: "jorMandanten");

            migrationBuilder.DropColumn(
                name: "Steuernummer",
                table: "jorMandanten");

            migrationBuilder.DropColumn(
                name: "Strasse",
                table: "jorMandanten");

            migrationBuilder.DropColumn(
                name: "Telefon",
                table: "jorMandanten");

            migrationBuilder.DropColumn(
                name: "UstIdNr",
                table: "jorMandanten");
        }
    }
}
