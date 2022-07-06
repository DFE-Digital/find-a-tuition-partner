using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class TuitionTypeInSchool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TuitionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "In School");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TuitionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "In Person");
        }
    }
}
