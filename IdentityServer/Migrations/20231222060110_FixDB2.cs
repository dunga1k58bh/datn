using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityServer.Migrations
{
    public partial class FixDB2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiResourcePropertys_ApiResources_ApiResourceId",
                table: "ApiResourcePropertys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiResourcePropertys",
                table: "ApiResourcePropertys");

            migrationBuilder.RenameTable(
                name: "ApiResourcePropertys",
                newName: "ApiResourceProperties");

            migrationBuilder.RenameIndex(
                name: "IX_ApiResourcePropertys_ApiResourceId",
                table: "ApiResourceProperties",
                newName: "IX_ApiResourceProperties_ApiResourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiResourceProperties",
                table: "ApiResourceProperties",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiResourceProperties_ApiResources_ApiResourceId",
                table: "ApiResourceProperties",
                column: "ApiResourceId",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiResourceProperties_ApiResources_ApiResourceId",
                table: "ApiResourceProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiResourceProperties",
                table: "ApiResourceProperties");

            migrationBuilder.RenameTable(
                name: "ApiResourceProperties",
                newName: "ApiResourcePropertys");

            migrationBuilder.RenameIndex(
                name: "IX_ApiResourceProperties_ApiResourceId",
                table: "ApiResourcePropertys",
                newName: "IX_ApiResourcePropertys_ApiResourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiResourcePropertys",
                table: "ApiResourcePropertys",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiResourcePropertys_ApiResources_ApiResourceId",
                table: "ApiResourcePropertys",
                column: "ApiResourceId",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
