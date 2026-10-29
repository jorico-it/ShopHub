using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopifySQL.Web.Migrations
{
    /// <inheritdoc />
    public partial class VariantenmerkmaleJsonTyp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Artikelvariante_Merkmale_JSON",
                table: "jorArtikelvarianten");

            migrationBuilder.AlterColumn<string>(
                name: "Variantenmerkmale",
                table: "jorArtikelvarianten",
                type: "json",
                nullable: false,
                defaultValueSql: "N'{}'",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValueSql: "N'{}'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Variantenmerkmale",
                table: "jorArtikelvarianten",
                type: "nvarchar(max)",
                nullable: false,
                defaultValueSql: "N'{}'",
                oldClrType: typeof(string),
                oldType: "json",
                oldDefaultValueSql: "N'{}'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Artikelvariante_Merkmale_JSON",
                table: "jorArtikelvarianten",
                sql: "ISJSON([Variantenmerkmale]) = 1");
        }
    }
}
