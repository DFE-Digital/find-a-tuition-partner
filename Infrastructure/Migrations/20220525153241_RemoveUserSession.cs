using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class RemoveUserSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSearches_UserSessions_UserSessionId",
                table: "UserSearches");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropIndex(
                name: "IX_UserSearches_UserSessionId",
                table: "UserSearches");

            migrationBuilder.DropColumn(
                name: "UserSessionId",
                table: "UserSearches");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserSessionId",
                table: "UserSearches",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSearches_UserSessionId",
                table: "UserSearches",
                column: "UserSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSearches_UserSessions_UserSessionId",
                table: "UserSearches",
                column: "UserSessionId",
                principalTable: "UserSessions",
                principalColumn: "Id");
        }
    }
}
