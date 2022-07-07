using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class CoverageIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubjectCoverage_TuitionPartnerId",
                table: "SubjectCoverage");

            migrationBuilder.DropIndex(
                name: "IX_SubjectCoverage_TuitionTypeId",
                table: "SubjectCoverage");

            migrationBuilder.DropIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionPartnerId",
                table: "LocalAuthorityDistrictCoverage");

            migrationBuilder.DropIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionTypeId",
                table: "LocalAuthorityDistrictCoverage");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCoverage_TuitionPartnerId_SubjectId",
                table: "SubjectCoverage",
                columns: new[] { "TuitionPartnerId", "SubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCoverage_TuitionPartnerId_TuitionTypeId_SubjectId",
                table: "SubjectCoverage",
                columns: new[] { "TuitionPartnerId", "TuitionTypeId", "SubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCoverage_TuitionTypeId_SubjectId",
                table: "SubjectCoverage",
                columns: new[] { "TuitionTypeId", "SubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_Prices_GroupSize",
                table: "Prices",
                column: "GroupSize");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionPartnerId_LocalAuthor~",
                table: "LocalAuthorityDistrictCoverage",
                columns: new[] { "TuitionPartnerId", "LocalAuthorityDistrictId" });

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionPartnerId_TuitionType~",
                table: "LocalAuthorityDistrictCoverage",
                columns: new[] { "TuitionPartnerId", "TuitionTypeId", "LocalAuthorityDistrictId" });

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionTypeId_LocalAuthority~",
                table: "LocalAuthorityDistrictCoverage",
                columns: new[] { "TuitionTypeId", "LocalAuthorityDistrictId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubjectCoverage_TuitionPartnerId_SubjectId",
                table: "SubjectCoverage");

            migrationBuilder.DropIndex(
                name: "IX_SubjectCoverage_TuitionPartnerId_TuitionTypeId_SubjectId",
                table: "SubjectCoverage");

            migrationBuilder.DropIndex(
                name: "IX_SubjectCoverage_TuitionTypeId_SubjectId",
                table: "SubjectCoverage");

            migrationBuilder.DropIndex(
                name: "IX_Prices_GroupSize",
                table: "Prices");

            migrationBuilder.DropIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionPartnerId_LocalAuthor~",
                table: "LocalAuthorityDistrictCoverage");

            migrationBuilder.DropIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionPartnerId_TuitionType~",
                table: "LocalAuthorityDistrictCoverage");

            migrationBuilder.DropIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionTypeId_LocalAuthority~",
                table: "LocalAuthorityDistrictCoverage");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCoverage_TuitionPartnerId",
                table: "SubjectCoverage",
                column: "TuitionPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCoverage_TuitionTypeId",
                table: "SubjectCoverage",
                column: "TuitionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionPartnerId",
                table: "LocalAuthorityDistrictCoverage",
                column: "TuitionPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionTypeId",
                table: "LocalAuthorityDistrictCoverage",
                column: "TuitionTypeId");
        }
    }
}
