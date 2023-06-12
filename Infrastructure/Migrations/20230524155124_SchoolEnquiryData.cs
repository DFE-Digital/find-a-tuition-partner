using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class SchoolEnquiryData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstablishmentNumber",
                table: "Schools",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Schools",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Ukprn",
                table: "Schools",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "Enquiries",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enquiries_SchoolId",
                table: "Enquiries",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enquiries_Schools_SchoolId",
                table: "Enquiries",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enquiries_Schools_SchoolId",
                table: "Enquiries");

            migrationBuilder.DropIndex(
                name: "IX_Enquiries_SchoolId",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "EstablishmentNumber",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "Ukprn",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Enquiries");
        }
    }
}
