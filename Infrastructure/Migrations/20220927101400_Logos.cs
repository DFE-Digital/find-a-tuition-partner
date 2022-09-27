using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Logos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "TuitionPartners",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasLogo",
                table: "TuitionPartners",
                type: "boolean",
                nullable: false,
                computedColumnSql: "case when \"Logo\" is null then false else true end",
                stored: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasLogo",
                table: "TuitionPartners");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "TuitionPartners");
        }
    }
}
