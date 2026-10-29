using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopHub.Web.Migrations
{
    /// <inheritdoc />
    public partial class BundleElementeJsonTyp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Artikelvariante_BundleElemente_JSON",
                table: "jorArtikelvarianten");

            migrationBuilder.AlterColumn<string>(
                name: "BundleElemente",
                table: "jorArtikelvarianten",
                type: "json",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BundleElemente",
                table: "jorArtikelvarianten",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "json",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Artikelvariante_BundleElemente_JSON",
                table: "jorArtikelvarianten",
                sql: "[BundleElemente] IS NULL OR ISJSON([BundleElemente]) = 1");
        }
    }
}
