using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddEnquiryResponseMagicLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MagicLinkId",
                table: "EnquiryResponses",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryResponses_MagicLinkId",
                table: "EnquiryResponses",
                column: "MagicLinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_EnquiryResponses_MagicLinks_MagicLinkId",
                table: "EnquiryResponses",
                column: "MagicLinkId",
                principalTable: "MagicLinks",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnquiryResponses_MagicLinks_MagicLinkId",
                table: "EnquiryResponses");

            migrationBuilder.DropIndex(
                name: "IX_EnquiryResponses_MagicLinkId",
                table: "EnquiryResponses");

            migrationBuilder.DropColumn(
                name: "MagicLinkId",
                table: "EnquiryResponses");
        }
    }
}
