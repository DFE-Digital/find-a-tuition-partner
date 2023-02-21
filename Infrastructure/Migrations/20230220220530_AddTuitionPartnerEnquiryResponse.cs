using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddTuitionPartnerEnquiryResponse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnquiryResponseId",
                table: "TuitionPartnersEnquiry",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnersEnquiry_EnquiryResponseId",
                table: "TuitionPartnersEnquiry",
                column: "EnquiryResponseId");

            migrationBuilder.AddForeignKey(
                name: "FK_TuitionPartnersEnquiry_EnquiryResponses_EnquiryResponseId",
                table: "TuitionPartnersEnquiry",
                column: "EnquiryResponseId",
                principalTable: "EnquiryResponses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TuitionPartnersEnquiry_EnquiryResponses_EnquiryResponseId",
                table: "TuitionPartnersEnquiry");

            migrationBuilder.DropIndex(
                name: "IX_TuitionPartnersEnquiry_EnquiryResponseId",
                table: "TuitionPartnersEnquiry");

            migrationBuilder.DropColumn(
                name: "EnquiryResponseId",
                table: "TuitionPartnersEnquiry");
        }
    }
}
