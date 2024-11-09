using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecomm_Book.DataAccess.Migrations
{
    public partial class AdAppUserInOrderHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "MyOrderHeaders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MyOrderHeaders_ApplicationUserId",
                table: "MyOrderHeaders",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MyOrderHeaders_AspNetUsers_ApplicationUserId",
                table: "MyOrderHeaders",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyOrderHeaders_AspNetUsers_ApplicationUserId",
                table: "MyOrderHeaders");

            migrationBuilder.DropIndex(
                name: "IX_MyOrderHeaders_ApplicationUserId",
                table: "MyOrderHeaders");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "MyOrderHeaders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
