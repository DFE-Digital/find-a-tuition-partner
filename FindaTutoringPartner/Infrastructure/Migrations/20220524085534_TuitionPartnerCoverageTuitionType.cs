using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class TuitionPartnerCoverageTuitionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TuitionPartnerCoverage_LocalAuthorityDistrictId",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropIndex(
                name: "IX_TuitionPartnerCoverage_TuitionPartnerId",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropColumn(
                name: "InPerson",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropColumn(
                name: "Online",
                table: "TuitionPartnerCoverage");

            migrationBuilder.AddColumn<int>(
                name: "TuitionTypeId",
                table: "TuitionPartnerCoverage",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerCoverage_LocalAuthorityDistrictId_TuitionType~",
                table: "TuitionPartnerCoverage",
                columns: new[] { "LocalAuthorityDistrictId", "TuitionTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerCoverage_TuitionPartnerId_LocalAuthorityDistr~",
                table: "TuitionPartnerCoverage",
                columns: new[] { "TuitionPartnerId", "LocalAuthorityDistrictId", "TuitionTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerCoverage_TuitionTypeId",
                table: "TuitionPartnerCoverage",
                column: "TuitionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TuitionPartnerCoverage_TuitionTypes_TuitionTypeId",
                table: "TuitionPartnerCoverage",
                column: "TuitionTypeId",
                principalTable: "TuitionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TuitionPartnerCoverage_TuitionTypes_TuitionTypeId",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropIndex(
                name: "IX_TuitionPartnerCoverage_LocalAuthorityDistrictId_TuitionType~",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropIndex(
                name: "IX_TuitionPartnerCoverage_TuitionPartnerId_LocalAuthorityDistr~",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropIndex(
                name: "IX_TuitionPartnerCoverage_TuitionTypeId",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropColumn(
                name: "TuitionTypeId",
                table: "TuitionPartnerCoverage");

            migrationBuilder.AddColumn<bool>(
                name: "InPerson",
                table: "TuitionPartnerCoverage",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Online",
                table: "TuitionPartnerCoverage",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerCoverage_LocalAuthorityDistrictId",
                table: "TuitionPartnerCoverage",
                column: "LocalAuthorityDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerCoverage_TuitionPartnerId",
                table: "TuitionPartnerCoverage",
                column: "TuitionPartnerId");
        }
    }
}
