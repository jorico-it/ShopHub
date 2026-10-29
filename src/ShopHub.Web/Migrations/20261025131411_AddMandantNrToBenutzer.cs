using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopHub.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddMandantNrToBenutzer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MandantNr",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MandantNr",
                table: "AspNetUsers",
                column: "MandantNr");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Mandanten",
                table: "AspNetUsers",
                column: "MandantNr",
                principalTable: "Mandanten",
                principalColumn: "MandantNr");

            // Erster Mandant und Zuordnung der bereits vorhandenen Konten (Erstkonto-Admin).
            // ErstelltAm/GeaendertAm/Aktiv kommen aus den Spalten-Defaults.
            migrationBuilder.Sql(
                "INSERT INTO [Mandanten] ([MandantNr], [Bezeichnung]) VALUES (1, N'Einsdorf GmbH Good Products');");
            migrationBuilder.Sql(
                "UPDATE [AspNetUsers] SET [MandantNr] = 1 WHERE [MandantNr] IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Mandanten",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_MandantNr",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MandantNr",
                table: "AspNetUsers");
        }
    }
}
