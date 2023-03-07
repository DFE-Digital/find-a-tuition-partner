using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddSupportReferenceNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupportReferenceNumber",
                table: "Enquiries",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enquiries_SupportReferenceNumber",
                table: "Enquiries",
                column: "SupportReferenceNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Enquiries_SupportReferenceNumber",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "SupportReferenceNumber",
                table: "Enquiries");
        }
    }
}
