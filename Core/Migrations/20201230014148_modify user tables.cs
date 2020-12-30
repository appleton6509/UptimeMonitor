using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class modifyusertables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EndPoint_User_UserID",
                table: "EndPoint");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_EndPoint_UserID",
                table: "EndPoint");

            migrationBuilder.AddColumn<int>(
                name: "WebUserID",
                table: "EndPoint",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WebUser",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebUser", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EndPoint_WebUserID",
                table: "EndPoint",
                column: "WebUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_EndPoint_WebUser_WebUserID",
                table: "EndPoint",
                column: "WebUserID",
                principalTable: "WebUser",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EndPoint_WebUser_WebUserID",
                table: "EndPoint");

            migrationBuilder.DropTable(
                name: "WebUser");

            migrationBuilder.DropIndex(
                name: "IX_EndPoint_WebUserID",
                table: "EndPoint");

            migrationBuilder.DropColumn(
                name: "WebUserID",
                table: "EndPoint");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EndPoint_UserID",
                table: "EndPoint",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_EndPoint_User_UserID",
                table: "EndPoint",
                column: "UserID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
