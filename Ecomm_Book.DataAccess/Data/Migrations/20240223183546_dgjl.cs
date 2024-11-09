using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecomm_Book.DataAccess.Migrations
{
    public partial class dgjl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "AddressAnothers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AddressAnothers_ApplicationUserId",
                table: "AddressAnothers",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressAnothers_AspNetUsers_ApplicationUserId",
                table: "AddressAnothers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressAnothers_AspNetUsers_ApplicationUserId",
                table: "AddressAnothers");

            migrationBuilder.DropIndex(
                name: "IX_AddressAnothers_ApplicationUserId",
                table: "AddressAnothers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AddressAnothers");
        }
    }
}
