using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopHub.Web.Migrations
{
    /// <inheritdoc />
    public partial class PraefixJorFuerTabellen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "Marken",
                newName: "jorMarken");

            migrationBuilder.RenameTable(
                name: "Mandanten",
                newName: "jorMandanten");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "jorAspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "jorAspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "jorAspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "jorAspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "jorAspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "jorAspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "jorAspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "Artikelvariante",
                newName: "jorArtikelvariante");

            migrationBuilder.RenameTable(
                name: "Artikel",
                newName: "jorArtikel");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_MandantNr",
                table: "jorAspNetUsers",
                newName: "IX_jorAspNetUsers_MandantNr");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "jorAspNetUserRoles",
                newName: "IX_jorAspNetUserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "jorAspNetUserLogins",
                newName: "IX_jorAspNetUserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "jorAspNetUserClaims",
                newName: "IX_jorAspNetUserClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "jorAspNetRoleClaims",
                newName: "IX_jorAspNetRoleClaims_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Artikelvariante_MandantNr_ArtikelID",
                table: "jorArtikelvariante",
                newName: "IX_jorArtikelvariante_MandantNr_ArtikelID");

            migrationBuilder.RenameIndex(
                name: "IX_Artikel_MandantNr_MarkeID",
                table: "jorArtikel",
                newName: "IX_jorArtikel_MandantNr_MarkeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_jorAspNetUserTokens",
                table: "jorAspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_jorAspNetUsers",
                table: "jorAspNetUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_jorAspNetUserRoles",
                table: "jorAspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_jorAspNetUserLogins",
                table: "jorAspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_jorAspNetUserClaims",
                table: "jorAspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_jorAspNetRoles",
                table: "jorAspNetRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_jorAspNetRoleClaims",
                table: "jorAspNetRoleClaims",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_jorAspNetRoleClaims_jorAspNetRoles_RoleId",
                table: "jorAspNetRoleClaims",
                column: "RoleId",
                principalTable: "jorAspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_jorAspNetUserClaims_jorAspNetUsers_UserId",
                table: "jorAspNetUserClaims",
                column: "UserId",
                principalTable: "jorAspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_jorAspNetUserLogins_jorAspNetUsers_UserId",
                table: "jorAspNetUserLogins",
                column: "UserId",
                principalTable: "jorAspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_jorAspNetUserRoles_jorAspNetRoles_RoleId",
                table: "jorAspNetUserRoles",
                column: "RoleId",
                principalTable: "jorAspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_jorAspNetUserRoles_jorAspNetUsers_UserId",
                table: "jorAspNetUserRoles",
                column: "UserId",
                principalTable: "jorAspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_jorAspNetUserTokens_jorAspNetUsers_UserId",
                table: "jorAspNetUserTokens",
                column: "UserId",
                principalTable: "jorAspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_jorAspNetRoleClaims_jorAspNetRoles_RoleId",
                table: "jorAspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_jorAspNetUserClaims_jorAspNetUsers_UserId",
                table: "jorAspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_jorAspNetUserLogins_jorAspNetUsers_UserId",
                table: "jorAspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_jorAspNetUserRoles_jorAspNetRoles_RoleId",
                table: "jorAspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_jorAspNetUserRoles_jorAspNetUsers_UserId",
                table: "jorAspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_jorAspNetUserTokens_jorAspNetUsers_UserId",
                table: "jorAspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_jorAspNetUserTokens",
                table: "jorAspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_jorAspNetUsers",
                table: "jorAspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_jorAspNetUserRoles",
                table: "jorAspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_jorAspNetUserLogins",
                table: "jorAspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_jorAspNetUserClaims",
                table: "jorAspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_jorAspNetRoles",
                table: "jorAspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_jorAspNetRoleClaims",
                table: "jorAspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "jorMarken",
                newName: "Marken");

            migrationBuilder.RenameTable(
                name: "jorMandanten",
                newName: "Mandanten");

            migrationBuilder.RenameTable(
                name: "jorAspNetUserTokens",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "jorAspNetUsers",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "jorAspNetUserRoles",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "jorAspNetUserLogins",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "jorAspNetUserClaims",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "jorAspNetRoles",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "jorAspNetRoleClaims",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "jorArtikelvariante",
                newName: "Artikelvariante");

            migrationBuilder.RenameTable(
                name: "jorArtikel",
                newName: "Artikel");

            migrationBuilder.RenameIndex(
                name: "IX_jorAspNetUsers_MandantNr",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_MandantNr");

            migrationBuilder.RenameIndex(
                name: "IX_jorAspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_jorAspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_jorAspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_jorAspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                newName: "IX_AspNetRoleClaims_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_jorArtikelvariante_MandantNr_ArtikelID",
                table: "Artikelvariante",
                newName: "IX_Artikelvariante_MandantNr_ArtikelID");

            migrationBuilder.RenameIndex(
                name: "IX_jorArtikel_MandantNr_MarkeID",
                table: "Artikel",
                newName: "IX_Artikel_MandantNr_MarkeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
