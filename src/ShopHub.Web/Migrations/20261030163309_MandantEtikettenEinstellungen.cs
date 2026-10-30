using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopHub.Web.Migrations
{
    /// <inheritdoc />
    public partial class MandantEtikettenEinstellungen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EtikettBreiteMm",
                table: "jorMandanten",
                type: "int",
                nullable: false,
                defaultValue: 152);

            migrationBuilder.AddColumn<int>(
                name: "EtikettHoeheMm",
                table: "jorMandanten",
                type: "int",
                nullable: false,
                defaultValue: 103);

            migrationBuilder.AddColumn<int>(
                name: "EtikettenProReihe",
                table: "jorMandanten",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EtikettBreiteMm",
                table: "jorMandanten");

            migrationBuilder.DropColumn(
                name: "EtikettHoeheMm",
                table: "jorMandanten");

            migrationBuilder.DropColumn(
                name: "EtikettenProReihe",
                table: "jorMandanten");
        }
    }
}
