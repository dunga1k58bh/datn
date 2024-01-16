using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityServer.Migrations
{
    public partial class FixDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityResourcePropertys_IdentityResources_IdentityResourceId",
                table: "IdentityResourcePropertys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityResourcePropertys",
                table: "IdentityResourcePropertys");

            migrationBuilder.RenameTable(
                name: "IdentityResourcePropertys",
                newName: "IdentityResourceProperties");

            migrationBuilder.RenameIndex(
                name: "IX_IdentityResourcePropertys_IdentityResourceId",
                table: "IdentityResourceProperties",
                newName: "IX_IdentityResourceProperties_IdentityResourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityResourceProperties",
                table: "IdentityResourceProperties",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityResourceProperties_IdentityResources_IdentityResourceId",
                table: "IdentityResourceProperties",
                column: "IdentityResourceId",
                principalTable: "IdentityResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityResourceProperties_IdentityResources_IdentityResourceId",
                table: "IdentityResourceProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityResourceProperties",
                table: "IdentityResourceProperties");

            migrationBuilder.RenameTable(
                name: "IdentityResourceProperties",
                newName: "IdentityResourcePropertys");

            migrationBuilder.RenameIndex(
                name: "IX_IdentityResourceProperties_IdentityResourceId",
                table: "IdentityResourcePropertys",
                newName: "IX_IdentityResourcePropertys_IdentityResourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityResourcePropertys",
                table: "IdentityResourcePropertys",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityResourcePropertys_IdentityResources_IdentityResourceId",
                table: "IdentityResourcePropertys",
                column: "IdentityResourceId",
                principalTable: "IdentityResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
