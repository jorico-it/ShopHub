using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopHub.Web.Migrations
{
    /// <inheritdoc />
    public partial class VariantenFelderBundleLagerBestand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BundleElemente",
                table: "jorArtikelvarianten",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HsCode",
                table: "jorArtikelvarianten",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lieferant",
                table: "jorArtikelvarianten",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Mengen",
                table: "jorArtikelvarianten",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProduktHoehe",
                table: "jorArtikelvarianten",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProduktLaenge",
                table: "jorArtikelvarianten",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SkuLieferant",
                table: "jorArtikelvarianten",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ursprungsland",
                table: "jorArtikelvarianten",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "UQ_Artikelvariante_Mandant_ID",
                table: "jorArtikelvarianten",
                columns: new[] { "MandantNr", "ArtikelvarianteID" });

            migrationBuilder.CreateTable(
                name: "jorLagerplaetze",
                columns: table => new
                {
                    LagerplatzID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MandantNr = table.Column<int>(type: "int", nullable: false),
                    Bezeichnung = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ErstelltAm = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    GeaendertAm = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lagerplaetze", x => x.LagerplatzID);
                    table.UniqueConstraint("UQ_Lagerplaetze_Mandant_ID", x => new { x.MandantNr, x.LagerplatzID });
                    table.ForeignKey(
                        name: "FK_Lagerplaetze_Mandanten",
                        column: x => x.MandantNr,
                        principalTable: "jorMandanten",
                        principalColumn: "MandantNr");
                });

            migrationBuilder.CreateTable(
                name: "jorArtikelBestaende",
                columns: table => new
                {
                    ArtikelBestandID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MandantNr = table.Column<int>(type: "int", nullable: false),
                    ArtikelvarianteID = table.Column<int>(type: "int", nullable: false),
                    LagerplatzID = table.Column<int>(type: "int", nullable: false),
                    Bestand = table.Column<int>(type: "int", nullable: false),
                    ErstelltAm = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    GeaendertAm = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtikelBestaende", x => x.ArtikelBestandID);
                    table.ForeignKey(
                        name: "FK_ArtikelBestaende_Artikelvarianten",
                        columns: x => new { x.MandantNr, x.ArtikelvarianteID },
                        principalTable: "jorArtikelvarianten",
                        principalColumns: new[] { "MandantNr", "ArtikelvarianteID" });
                    table.ForeignKey(
                        name: "FK_ArtikelBestaende_Lagerplaetze",
                        columns: x => new { x.MandantNr, x.LagerplatzID },
                        principalTable: "jorLagerplaetze",
                        principalColumns: new[] { "MandantNr", "LagerplatzID" });
                    table.ForeignKey(
                        name: "FK_ArtikelBestaende_Mandanten",
                        column: x => x.MandantNr,
                        principalTable: "jorMandanten",
                        principalColumn: "MandantNr");
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Artikelvariante_BundleElemente_JSON",
                table: "jorArtikelvarianten",
                sql: "[BundleElemente] IS NULL OR ISJSON([BundleElemente]) = 1");

            migrationBuilder.CreateIndex(
                name: "IX_jorArtikelBestaende_MandantNr_LagerplatzID",
                table: "jorArtikelBestaende",
                columns: new[] { "MandantNr", "LagerplatzID" });

            migrationBuilder.CreateIndex(
                name: "UQ_ArtikelBestaende_Mandant_Variante_Lagerplatz",
                table: "jorArtikelBestaende",
                columns: new[] { "MandantNr", "ArtikelvarianteID", "LagerplatzID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Lagerplaetze_Mandant_Bezeichnung",
                table: "jorLagerplaetze",
                columns: new[] { "MandantNr", "Bezeichnung" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jorArtikelBestaende");

            migrationBuilder.DropTable(
                name: "jorLagerplaetze");

            migrationBuilder.DropUniqueConstraint(
                name: "UQ_Artikelvariante_Mandant_ID",
                table: "jorArtikelvarianten");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Artikelvariante_BundleElemente_JSON",
                table: "jorArtikelvarianten");

            migrationBuilder.DropColumn(
                name: "BundleElemente",
                table: "jorArtikelvarianten");

            migrationBuilder.DropColumn(
                name: "HsCode",
                table: "jorArtikelvarianten");

            migrationBuilder.DropColumn(
                name: "Lieferant",
                table: "jorArtikelvarianten");

            migrationBuilder.DropColumn(
                name: "Mengen",
                table: "jorArtikelvarianten");

            migrationBuilder.DropColumn(
                name: "ProduktHoehe",
                table: "jorArtikelvarianten");

            migrationBuilder.DropColumn(
                name: "ProduktLaenge",
                table: "jorArtikelvarianten");

            migrationBuilder.DropColumn(
                name: "SkuLieferant",
                table: "jorArtikelvarianten");

            migrationBuilder.DropColumn(
                name: "Ursprungsland",
                table: "jorArtikelvarianten");
        }
    }
}
