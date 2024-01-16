using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityServer.Migrations
{
    public partial class FixDB3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientPropertys_Clients_ClientId",
                table: "ClientPropertys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientPropertys",
                table: "ClientPropertys");

            migrationBuilder.RenameTable(
                name: "ClientPropertys",
                newName: "ClientProperties");

            migrationBuilder.RenameIndex(
                name: "IX_ClientPropertys_ClientId",
                table: "ClientProperties",
                newName: "IX_ClientProperties_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientProperties",
                table: "ClientProperties",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientProperties_Clients_ClientId",
                table: "ClientProperties",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientProperties_Clients_ClientId",
                table: "ClientProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientProperties",
                table: "ClientProperties");

            migrationBuilder.RenameTable(
                name: "ClientProperties",
                newName: "ClientPropertys");

            migrationBuilder.RenameIndex(
                name: "IX_ClientProperties_ClientId",
                table: "ClientPropertys",
                newName: "IX_ClientPropertys_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientPropertys",
                table: "ClientPropertys",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientPropertys_Clients_ClientId",
                table: "ClientPropertys",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
