using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddTuitionPartnerEnquiryMagicLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MagicLinkId",
                table: "TuitionPartnersEnquiry",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnersEnquiry_MagicLinkId",
                table: "TuitionPartnersEnquiry",
                column: "MagicLinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_TuitionPartnersEnquiry_MagicLinks_MagicLinkId",
                table: "TuitionPartnersEnquiry",
                column: "MagicLinkId",
                principalTable: "MagicLinks",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TuitionPartnersEnquiry_MagicLinks_MagicLinkId",
                table: "TuitionPartnersEnquiry");

            migrationBuilder.DropIndex(
                name: "IX_TuitionPartnersEnquiry_MagicLinkId",
                table: "TuitionPartnersEnquiry");

            migrationBuilder.DropColumn(
                name: "MagicLinkId",
                table: "TuitionPartnersEnquiry");
        }
    }
}
