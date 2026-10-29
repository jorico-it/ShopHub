using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopifySQL.Web.Migrations
{
    /// <inheritdoc />
    public partial class ErstanmeldungPasswortUndBesucherLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MussPasswortAendern",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "jorBesucherLog",
                columns: table => new
                {
                    BesucherLogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ErstelltAm = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    Pfad = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IpAdresse = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    MandantNr = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BesucherLog", x => x.BesucherLogID);
                    table.ForeignKey(
                        name: "FK_BesucherLog_Mandanten",
                        column: x => x.MandantNr,
                        principalTable: "jorMandanten",
                        principalColumn: "MandantNr");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BesucherLog_ErstelltAm",
                table: "jorBesucherLog",
                column: "ErstelltAm");

            migrationBuilder.CreateIndex(
                name: "IX_jorBesucherLog_MandantNr",
                table: "jorBesucherLog",
                column: "MandantNr");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jorBesucherLog");

            migrationBuilder.DropColumn(
                name: "MussPasswortAendern",
                table: "AspNetUsers");
        }
    }
}
