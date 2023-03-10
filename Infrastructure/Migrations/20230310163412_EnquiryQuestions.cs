using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class EnquiryQuestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnquiryText",
                table: "Enquiries",
                newName: "TutoringLogistics");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInformation",
                table: "Enquiries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SENDRequirements",
                table: "Enquiries",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalInformation",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "SENDRequirements",
                table: "Enquiries");

            migrationBuilder.RenameColumn(
                name: "TutoringLogistics",
                table: "Enquiries",
                newName: "EnquiryText");
        }
    }
}
