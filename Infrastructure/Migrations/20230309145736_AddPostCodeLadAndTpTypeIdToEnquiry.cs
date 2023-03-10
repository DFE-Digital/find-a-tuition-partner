using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddPostCodeLadAndTpTypeIdToEnquiry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocalAuthorityDistrict",
                table: "Enquiries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                table: "Enquiries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TuitionTypeId",
                table: "Enquiries",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalAuthorityDistrict",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "TuitionTypeId",
                table: "Enquiries");
        }
    }
}
