using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class additionalmodificationstowebuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EndPoint_WebUser_WebUserID",
                table: "EndPoint");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "WebUser",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "WebUserID",
                table: "EndPoint",
                newName: "WebUserId");

            migrationBuilder.RenameIndex(
                name: "IX_EndPoint_WebUserID",
                table: "EndPoint",
                newName: "IX_EndPoint_WebUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EndPoint_WebUser_WebUserId",
                table: "EndPoint",
                column: "WebUserId",
                principalTable: "WebUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EndPoint_WebUser_WebUserId",
                table: "EndPoint");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "WebUser",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "WebUserId",
                table: "EndPoint",
                newName: "WebUserID");

            migrationBuilder.RenameIndex(
                name: "IX_EndPoint_WebUserId",
                table: "EndPoint",
                newName: "IX_EndPoint_WebUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_EndPoint_WebUser_WebUserID",
                table: "EndPoint",
                column: "WebUserID",
                principalTable: "WebUser",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
