using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class renamehttptohttpresult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Http_EndPoint_EndPointId",
                table: "Http");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Http",
                table: "Http");

            migrationBuilder.RenameTable(
                name: "Http",
                newName: "HttpResult");

            migrationBuilder.RenameIndex(
                name: "IX_Http_EndPointId",
                table: "HttpResult",
                newName: "IX_HttpResult_EndPointId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HttpResult",
                table: "HttpResult",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HttpResult_EndPoint_EndPointId",
                table: "HttpResult",
                column: "EndPointId",
                principalTable: "EndPoint",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HttpResult_EndPoint_EndPointId",
                table: "HttpResult");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HttpResult",
                table: "HttpResult");

            migrationBuilder.RenameTable(
                name: "HttpResult",
                newName: "Http");

            migrationBuilder.RenameIndex(
                name: "IX_HttpResult_EndPointId",
                table: "Http",
                newName: "IX_Http_EndPointId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Http",
                table: "Http",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Http_EndPoint_EndPointId",
                table: "Http",
                column: "EndPointId",
                principalTable: "EndPoint",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
