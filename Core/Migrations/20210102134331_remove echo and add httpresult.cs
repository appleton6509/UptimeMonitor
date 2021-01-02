using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class removeechoandaddhttpresult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Echo");

            migrationBuilder.CreateTable(
                name: "Http",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsReachable = table.Column<bool>(type: "bit", nullable: false),
                    Latency = table.Column<int>(type: "int", nullable: false),
                    StatusMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndPointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Http", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Http_EndPoint_EndPointId",
                        column: x => x.EndPointId,
                        principalTable: "EndPoint",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Http_EndPointId",
                table: "Http",
                column: "EndPointId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Http");

            migrationBuilder.CreateTable(
                name: "Echo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EndPointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latency = table.Column<int>(type: "int", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    StatusMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Echo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Echo_EndPoint_EndPointId",
                        column: x => x.EndPointId,
                        principalTable: "EndPoint",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Echo_EndPointId",
                table: "Echo",
                column: "EndPointId");
        }
    }
}
