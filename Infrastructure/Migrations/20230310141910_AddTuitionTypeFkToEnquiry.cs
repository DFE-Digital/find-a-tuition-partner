using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddTuitionTypeFkToEnquiry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Enquiries_TuitionTypeId",
                table: "Enquiries",
                column: "TuitionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enquiries_TuitionTypes_TuitionTypeId",
                table: "Enquiries",
                column: "TuitionTypeId",
                principalTable: "TuitionTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enquiries_TuitionTypes_TuitionTypeId",
                table: "Enquiries");

            migrationBuilder.DropIndex(
                name: "IX_Enquiries_TuitionTypeId",
                table: "Enquiries");
        }
    }
}
