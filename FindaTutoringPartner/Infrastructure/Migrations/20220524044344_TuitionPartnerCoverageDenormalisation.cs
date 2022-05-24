using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class TuitionPartnerCoverageDenormalisation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TuitionPartnerCoverage_Subjects_SubjectId",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropForeignKey(
                name: "FK_TuitionPartnerCoverage_TuitionTypes_TuitionTypeId",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropIndex(
                name: "IX_TuitionPartnerCoverage_SubjectId",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropIndex(
                name: "IX_TuitionPartnerCoverage_TuitionTypeId",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropColumn(
                name: "SubjectId",
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

            migrationBuilder.AddColumn<bool>(
                name: "PrimaryLiteracy",
                table: "TuitionPartnerCoverage",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PrimaryNumeracy",
                table: "TuitionPartnerCoverage",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PrimaryScience",
                table: "TuitionPartnerCoverage",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SecondaryEnglish",
                table: "TuitionPartnerCoverage",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SecondaryHumanities",
                table: "TuitionPartnerCoverage",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SecondaryMaths",
                table: "TuitionPartnerCoverage",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SecondaryModernForeignLanguages",
                table: "TuitionPartnerCoverage",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SecondaryScience",
                table: "TuitionPartnerCoverage",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InPerson",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropColumn(
                name: "Online",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropColumn(
                name: "PrimaryLiteracy",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropColumn(
                name: "PrimaryNumeracy",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropColumn(
                name: "PrimaryScience",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropColumn(
                name: "SecondaryEnglish",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropColumn(
                name: "SecondaryHumanities",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropColumn(
                name: "SecondaryMaths",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropColumn(
                name: "SecondaryModernForeignLanguages",
                table: "TuitionPartnerCoverage");

            migrationBuilder.DropColumn(
                name: "SecondaryScience",
                table: "TuitionPartnerCoverage");

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "TuitionPartnerCoverage",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TuitionTypeId",
                table: "TuitionPartnerCoverage",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerCoverage_SubjectId",
                table: "TuitionPartnerCoverage",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TuitionPartnerCoverage_TuitionTypeId",
                table: "TuitionPartnerCoverage",
                column: "TuitionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TuitionPartnerCoverage_Subjects_SubjectId",
                table: "TuitionPartnerCoverage",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TuitionPartnerCoverage_TuitionTypes_TuitionTypeId",
                table: "TuitionPartnerCoverage",
                column: "TuitionTypeId",
                principalTable: "TuitionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
