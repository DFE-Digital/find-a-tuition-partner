using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class LocalAuthorityDataFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "LocalAuthorities",
                keyColumn: "Id",
                keyValue: 51,
                column: "Code",
                value: "E08000031");

            migrationBuilder.UpdateData(
                table: "LocalAuthorities",
                keyColumn: "Id",
                keyValue: 200,
                column: "Name",
                value: "Tower Hamlets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "LocalAuthorities",
                keyColumn: "Id",
                keyValue: 51,
                column: "Code",
                value: "E08000030");

            migrationBuilder.UpdateData(
                table: "LocalAuthorities",
                keyColumn: "Id",
                keyValue: 200,
                column: "Name",
                value: "Sutton");
        }
    }
}
