using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopHub.Web.Migrations
{
    /// <inheritdoc />
    public partial class ArtikelvariantenUmbenennenGtinJeMandant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Artikelvariante_GTIN",
                table: "jorArtikelvariante");

            migrationBuilder.RenameTable(
                name: "jorArtikelvariante",
                newName: "jorArtikelvarianten");

            migrationBuilder.RenameIndex(
                name: "IX_jorArtikelvariante_MandantNr_ArtikelID",
                table: "jorArtikelvarianten",
                newName: "IX_jorArtikelvarianten_MandantNr_ArtikelID");

            migrationBuilder.CreateIndex(
                name: "UX_Artikelvariante_Mandant_GTIN",
                table: "jorArtikelvarianten",
                columns: new[] { "MandantNr", "GTIN" },
                unique: true,
                filter: "[GTIN] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Artikelvariante_Mandant_GTIN",
                table: "jorArtikelvarianten");

            migrationBuilder.RenameTable(
                name: "jorArtikelvarianten",
                newName: "jorArtikelvariante");

            migrationBuilder.RenameIndex(
                name: "IX_jorArtikelvarianten_MandantNr_ArtikelID",
                table: "jorArtikelvariante",
                newName: "IX_jorArtikelvariante_MandantNr_ArtikelID");

            migrationBuilder.CreateIndex(
                name: "UX_Artikelvariante_GTIN",
                table: "jorArtikelvariante",
                column: "GTIN",
                unique: true,
                filter: "[GTIN] IS NOT NULL");
        }
    }
}
