using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddDeleteBehaviorCascadeEnquiryMagicLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MagicLinks_Enquiries_EnquiryId",
                table: "MagicLinks");

            migrationBuilder.AddColumn<int>(
                name: "EnquiryId1",
                table: "MagicLinks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MagicLinks_EnquiryId1",
                table: "MagicLinks",
                column: "EnquiryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_MagicLinks_Enquiries_EnquiryId",
                table: "MagicLinks",
                column: "EnquiryId",
                principalTable: "Enquiries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MagicLinks_Enquiries_EnquiryId1",
                table: "MagicLinks",
                column: "EnquiryId1",
                principalTable: "Enquiries",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MagicLinks_Enquiries_EnquiryId",
                table: "MagicLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_MagicLinks_Enquiries_EnquiryId1",
                table: "MagicLinks");

            migrationBuilder.DropIndex(
                name: "IX_MagicLinks_EnquiryId1",
                table: "MagicLinks");

            migrationBuilder.DropColumn(
                name: "EnquiryId1",
                table: "MagicLinks");

            migrationBuilder.AddForeignKey(
                name: "FK_MagicLinks_Enquiries_EnquiryId",
                table: "MagicLinks",
                column: "EnquiryId",
                principalTable: "Enquiries",
                principalColumn: "Id");
        }
    }
}
