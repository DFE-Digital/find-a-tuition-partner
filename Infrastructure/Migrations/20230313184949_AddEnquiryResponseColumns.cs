using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddEnquiryResponseColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnquiryResponseText",
                table: "EnquiryResponses",
                newName: "TutoringLogisticsText");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInformationText",
                table: "EnquiryResponses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyStageAndSubjectsText",
                table: "EnquiryResponses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SENDRequirementsText",
                table: "EnquiryResponses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TuitionTypeText",
                table: "EnquiryResponses",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalInformationText",
                table: "EnquiryResponses");

            migrationBuilder.DropColumn(
                name: "KeyStageAndSubjectsText",
                table: "EnquiryResponses");

            migrationBuilder.DropColumn(
                name: "SENDRequirementsText",
                table: "EnquiryResponses");

            migrationBuilder.DropColumn(
                name: "TuitionTypeText",
                table: "EnquiryResponses");

            migrationBuilder.RenameColumn(
                name: "TutoringLogisticsText",
                table: "EnquiryResponses",
                newName: "EnquiryResponseText");
        }
    }
}
