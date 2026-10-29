using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopHub.Web.Migrations
{
    /// <inheritdoc />
    public partial class MandantHrNummerAmtsgerichtLucidId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HrbNummer",
                table: "jorMandanten",
                newName: "HrNummer");

            migrationBuilder.AddColumn<string>(
                name: "Amtsgericht",
                table: "jorMandanten",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LucidId",
                table: "jorMandanten",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amtsgericht",
                table: "jorMandanten");

            migrationBuilder.DropColumn(
                name: "LucidId",
                table: "jorMandanten");

            migrationBuilder.RenameColumn(
                name: "HrNummer",
                table: "jorMandanten",
                newName: "HrbNummer");
        }
    }
}
