using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopifySQL.Web.Migrations
{
    /// <inheritdoc />
    public partial class MarkeLogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Logo",
                table: "jorMarken",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoContentType",
                table: "jorMarken",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "jorMarken");

            migrationBuilder.DropColumn(
                name: "LogoContentType",
                table: "jorMarken");
        }
    }
}
