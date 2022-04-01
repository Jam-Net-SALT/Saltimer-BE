using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saltimer.Api.Migrations
{
    public partial class UpdateUserMobSessionTableToSessionMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMobSession");

            migrationBuilder.CreateTable(
                name: "SessionMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Turn = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionMember_MobTimerSession_SessionId",
                        column: x => x.SessionId,
                        principalTable: "MobTimerSession",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SessionMember_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionMember_SessionId",
                table: "SessionMember",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionMember_UserId",
                table: "SessionMember",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionMember");

            migrationBuilder.CreateTable(
                name: "UserMobSession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MobTimerId = table.Column<int>(type: "int", nullable: false),
                    MobTimerSessionId = table.Column<int>(type: "int", nullable: true),
                    Turn = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMobSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMobSession_MobTimerSession_MobTimerSessionId",
                        column: x => x.MobTimerSessionId,
                        principalTable: "MobTimerSession",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMobSession_MobTimerSessionId",
                table: "UserMobSession",
                column: "MobTimerSessionId");
        }
    }
}
