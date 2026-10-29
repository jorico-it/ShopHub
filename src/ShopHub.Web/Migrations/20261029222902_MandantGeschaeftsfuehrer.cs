using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopHub.Web.Migrations
{
    /// <inheritdoc />
    public partial class MandantGeschaeftsfuehrer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Geschaeftsfuehrer",
                table: "jorMandanten",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Geschaeftsfuehrer",
                table: "jorMandanten");
        }
    }
}
