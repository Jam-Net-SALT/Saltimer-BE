using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saltimer.Api.Migrations
{
    public partial class UpdateMobTimerTableToMobTimerSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMobSession_MobTimer_MobTimerId",
                table: "UserMobSession");

            migrationBuilder.DropTable(
                name: "MobTimer");

            migrationBuilder.DropIndex(
                name: "IX_UserMobSession_MobTimerId",
                table: "UserMobSession");

            migrationBuilder.AddColumn<int>(
                name: "MobTimerSessionId",
                table: "UserMobSession",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MobTimerSession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoundTime = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BreakTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PausedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobTimerSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MobTimerSession_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMobSession_MobTimerSessionId",
                table: "UserMobSession",
                column: "MobTimerSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_MobTimerSession_OwnerId",
                table: "MobTimerSession",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMobSession_MobTimerSession_MobTimerSessionId",
                table: "UserMobSession",
                column: "MobTimerSessionId",
                principalTable: "MobTimerSession",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMobSession_MobTimerSession_MobTimerSessionId",
                table: "UserMobSession");

            migrationBuilder.DropTable(
                name: "MobTimerSession");

            migrationBuilder.DropIndex(
                name: "IX_UserMobSession_MobTimerSessionId",
                table: "UserMobSession");

            migrationBuilder.DropColumn(
                name: "MobTimerSessionId",
                table: "UserMobSession");

            migrationBuilder.CreateTable(
                name: "MobTimer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BreakTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MobName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    PausedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoundTime = table.Column<int>(type: "int", nullable: false),
                    SessionUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobTimer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MobTimer_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMobSession_MobTimerId",
                table: "UserMobSession",
                column: "MobTimerId");

            migrationBuilder.CreateIndex(
                name: "IX_MobTimer_UserId",
                table: "MobTimer",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMobSession_MobTimer_MobTimerId",
                table: "UserMobSession",
                column: "MobTimerId",
                principalTable: "MobTimer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
