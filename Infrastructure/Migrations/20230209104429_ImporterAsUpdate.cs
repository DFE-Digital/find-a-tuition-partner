using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ImporterAsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Delete all the data - since data needs to be re-added by the import process
            migrationBuilder.Sql("DELETE FROM \"LocalAuthorityDistrictCoverage\";", true);
            migrationBuilder.Sql("DELETE FROM \"Prices\";", true);
            migrationBuilder.Sql("DELETE FROM \"SubjectCoverage\";", true);
            migrationBuilder.Sql("DELETE FROM \"TuitionPartners\";", true);

            migrationBuilder.AddColumn<string>(
                name: "ImportId",
                table: "TuitionPartners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TuitionPartners",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartners_ImportId",
                table: "TuitionPartners",
                column: "ImportId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartners_IsActive",
                table: "TuitionPartners",
                column: "IsActive");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TuitionPartners_ImportId",
                table: "TuitionPartners");

            migrationBuilder.DropIndex(
                name: "IX_TuitionPartners_IsActive",
                table: "TuitionPartners");

            migrationBuilder.DropColumn(
                name: "ImportId",
                table: "TuitionPartners");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TuitionPartners");
        }
    }
}
