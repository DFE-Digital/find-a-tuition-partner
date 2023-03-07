using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class MakeSupportRefNonNullAble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Delete all the enquiry data: We add this (SupportReferenceNumber) non-nullable column to the Enquiries table.

            migrationBuilder.Sql("DELETE FROM \"TuitionPartnersEnquiry\";", true);
            migrationBuilder.Sql("DELETE FROM \"EnquiryResponses\";", true);
            migrationBuilder.Sql("DELETE FROM \"MagicLinks\";", true);
            migrationBuilder.Sql("DELETE FROM \"Enquiries\";", true);

            migrationBuilder.AlterColumn<string>(
                name: "SupportReferenceNumber",
                table: "Enquiries",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SupportReferenceNumber",
                table: "Enquiries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
