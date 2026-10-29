using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopifySQL.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddMandantenMarkenArtikel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mandanten",
                columns: table => new
                {
                    MandantNr = table.Column<int>(type: "int", nullable: false),
                    Bezeichnung = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ErstelltAm = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    GeaendertAm = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mandanten", x => x.MandantNr);
                });

            migrationBuilder.CreateTable(
                name: "Marken",
                columns: table => new
                {
                    MarkeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MandantNr = table.Column<int>(type: "int", nullable: false),
                    Markenname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ErstelltAm = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    GeaendertAm = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marken", x => x.MarkeID);
                    table.UniqueConstraint("UQ_Marken_Mandant_MarkeID", x => new { x.MandantNr, x.MarkeID });
                    table.ForeignKey(
                        name: "FK_Marken_Mandanten",
                        column: x => x.MandantNr,
                        principalTable: "Mandanten",
                        principalColumn: "MandantNr");
                });

            migrationBuilder.CreateTable(
                name: "Artikel",
                columns: table => new
                {
                    ArtikelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MandantNr = table.Column<int>(type: "int", nullable: false),
                    Artikelnummer = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Bezeichnung = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MarkeID = table.Column<int>(type: "int", nullable: true),
                    Kurzbeschreibung = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Beschreibung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mehrwertsteuersatz = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 19.00m),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ErstelltAm = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    GeaendertAm = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artikel", x => x.ArtikelID);
                    table.UniqueConstraint("UQ_Artikel_Mandant_ArtikelID", x => new { x.MandantNr, x.ArtikelID });
                    table.CheckConstraint("CK_Artikel_MwSt", "[Mehrwertsteuersatz] >= 0 AND [Mehrwertsteuersatz] <= 100");
                    table.ForeignKey(
                        name: "FK_Artikel_Mandanten",
                        column: x => x.MandantNr,
                        principalTable: "Mandanten",
                        principalColumn: "MandantNr");
                    table.ForeignKey(
                        name: "FK_Artikel_Marken",
                        columns: x => new { x.MandantNr, x.MarkeID },
                        principalTable: "Marken",
                        principalColumns: new[] { "MandantNr", "MarkeID" });
                });

            migrationBuilder.CreateTable(
                name: "Artikelvariante",
                columns: table => new
                {
                    ArtikelvarianteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MandantNr = table.Column<int>(type: "int", nullable: false),
                    ArtikelID = table.Column<int>(type: "int", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GTIN = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    Variantenname = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Variantenmerkmale = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "N'{}'"),
                    VerkaufspreisBrutto = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    EinkaufspreisNetto = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    GewichtGramm = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ErstelltAm = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    GeaendertAm = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artikelvariante", x => x.ArtikelvarianteID);
                    table.CheckConstraint("CK_Artikelvariante_Gewicht", "[GewichtGramm] IS NULL OR [GewichtGramm] >= 0");
                    table.CheckConstraint("CK_Artikelvariante_Merkmale_JSON", "ISJSON([Variantenmerkmale]) = 1");
                    table.CheckConstraint("CK_Artikelvariante_Preis", "[VerkaufspreisBrutto] >= 0");
                    table.ForeignKey(
                        name: "FK_Artikelvariante_Artikel",
                        columns: x => new { x.MandantNr, x.ArtikelID },
                        principalTable: "Artikel",
                        principalColumns: new[] { "MandantNr", "ArtikelID" });
                    table.ForeignKey(
                        name: "FK_Artikelvariante_Mandanten",
                        column: x => x.MandantNr,
                        principalTable: "Mandanten",
                        principalColumn: "MandantNr");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Artikel_MandantNr_MarkeID",
                table: "Artikel",
                columns: new[] { "MandantNr", "MarkeID" });

            migrationBuilder.CreateIndex(
                name: "UQ_Artikel_Mandant_Artikelnummer",
                table: "Artikel",
                columns: new[] { "MandantNr", "Artikelnummer" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artikelvariante_ArtikelID",
                table: "Artikelvariante",
                column: "ArtikelID");

            migrationBuilder.CreateIndex(
                name: "IX_Artikelvariante_MandantNr_ArtikelID",
                table: "Artikelvariante",
                columns: new[] { "MandantNr", "ArtikelID" });

            migrationBuilder.CreateIndex(
                name: "UQ_Artikelvariante_Mandant_SKU",
                table: "Artikelvariante",
                columns: new[] { "MandantNr", "SKU" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Artikelvariante_GTIN",
                table: "Artikelvariante",
                column: "GTIN",
                unique: true,
                filter: "[GTIN] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ_Marken_Mandant_Markenname",
                table: "Marken",
                columns: new[] { "MandantNr", "Markenname" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Artikelvariante");

            migrationBuilder.DropTable(
                name: "Artikel");

            migrationBuilder.DropTable(
                name: "Marken");

            migrationBuilder.DropTable(
                name: "Mandanten");
        }
    }
}
