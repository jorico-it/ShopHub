using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopHub.Web.Migrations
{
    /// <inheritdoc />
    public partial class AmazonAsinEindeutig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UX_Artikelvariante_Mandant_AmazonAsin",
                table: "jorArtikelvarianten",
                columns: new[] { "MandantNr", "AmazonAsin" },
                unique: true,
                filter: "[AmazonAsin] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Artikelvariante_Mandant_AmazonAsin",
                table: "jorArtikelvarianten");
        }
    }
}
