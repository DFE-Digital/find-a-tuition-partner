using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class TuitionPartnerCoverageAndImport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TuitionPartnerCoverage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TuitionPartnerId = table.Column<int>(type: "integer", nullable: false),
                    LocalAuthorityDistrictId = table.Column<int>(type: "integer", nullable: false),
                    TuitionTypeId = table.Column<int>(type: "integer", nullable: false),
                    PrimaryLiteracy = table.Column<bool>(type: "boolean", nullable: false),
                    PrimaryNumeracy = table.Column<bool>(type: "boolean", nullable: false),
                    PrimaryScience = table.Column<bool>(type: "boolean", nullable: false),
                    SecondaryEnglish = table.Column<bool>(type: "boolean", nullable: false),
                    SecondaryHumanities = table.Column<bool>(type: "boolean", nullable: false),
                    SecondaryMaths = table.Column<bool>(type: "boolean", nullable: false),
                    SecondaryModernForeignLanguages = table.Column<bool>(type: "boolean", nullable: false),
                    SecondaryScience = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuitionPartnerCoverage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TuitionPartnerCoverage_LocalAuthorityDistricts_LocalAuthori~",
                        column: x => x.LocalAuthorityDistrictId,
                        principalTable: "LocalAuthorityDistricts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TuitionPartnerCoverage_TuitionPartners_TuitionPartnerId",
                        column: x => x.TuitionPartnerId,
                        principalTable: "TuitionPartners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TuitionPartnerCoverage_TuitionTypes_TuitionTypeId",
                        column: x => x.TuitionTypeId,
                        principalTable: "TuitionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TuitionPartnerDataImportHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Importer = table.Column<string>(type: "text", nullable: false),
                    Md5Checksum = table.Column<string>(type: "text", nullable: false),
                    ImportDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuitionPartnerDataImportHistories", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Tutor Doctor Cambridge");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Career Tree");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "CoachBright");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 9,
                column: "Name",
                value: "Connex Education Partnership");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 10,
                column: "Name",
                value: "EM Tuition");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 13,
                column: "Name",
                value: "Equal Education Partners");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 15,
                column: "Name",
                value: "Fledge Tuition");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 16,
                column: "Name",
                value: "Fleet Education Services");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 19,
                column: "Name",
                value: "Manning's Tutors");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 21,
                column: "Name",
                value: "Pearson Education");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 22,
                column: "Name",
                value: "PET-XI Training");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 23,
                column: "Name",
                value: "Protocol Education");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 25,
                column: "Name",
                value: "Randstad Tuition Services");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 28,
                column: "Name",
                value: "Step Teachers");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 30,
                column: "Name",
                value: "Targeted Provision");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 41,
                column: "Name",
                value: "Seven Springs Education");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 43,
                column: "Name",
                value: "Nudge Education");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 45,
                column: "Name",
                value: "Bright Heart Education");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 46,
                column: "Name",
                value: "Tutor Doctor Bristol");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 47,
                column: "Name",
                value: "REESON Education");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 48,
                column: "Name",
                value: "3D Recruit");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 50,
                column: "Name",
                value: "Purple Ruler (ADM computing)");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 52,
                column: "Name",
                value: "Simply Learning Tuition");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 53,
                column: "Name",
                value: "Prospero Teaching");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 54,
                column: "Name",
                value: "Third Space Learning");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 56,
                column: "Name",
                value: "Assess Education");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TuitionPartnerCoverage");

            migrationBuilder.DropTable(
                name: "TuitionPartnerDataImportHistories");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Cambridge Tuition Limited");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Career Group Courses");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "Coachbright");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 9,
                column: "Name",
                value: "Connex Education Partnership Limited");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 10,
                column: "Name",
                value: "EM Skills Enterprise CIC (EM Tuition)");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 13,
                column: "Name",
                value: "Equal Education Partners (E-Qual)");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 15,
                column: "Name",
                value: "Fledge Tuition Ltd");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 16,
                column: "Name",
                value: "Fleet Education Services Limited");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 19,
                column: "Name",
                value: "Mannings Tutors");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 21,
                column: "Name",
                value: "Pearson Education Ltd");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 22,
                column: "Name",
                value: "Pet-Xi Training");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 23,
                column: "Name",
                value: "Protocol Education Ltd");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 25,
                column: "Name",
                value: "Randstad HR Solutions");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 28,
                column: "Name",
                value: "Step Teachers Limited");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 30,
                column: "Name",
                value: "Targeted provision Limited");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 41,
                column: "Name",
                value: "Nebula Education Ltd (Trading as Seven Springs Education)");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 43,
                column: "Name",
                value: "Nudge Education Limited");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 45,
                column: "Name",
                value: "Bright Heart Education Ltd");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 46,
                column: "Name",
                value: "Avon Education Services Ltd (Tutor Doctor Bristol)");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 47,
                column: "Name",
                value: "K&G Recruitment Consultancy Ltd t/a REESON Education");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 48,
                column: "Name",
                value: "3D Recruit Ltd");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 50,
                column: "Name",
                value: "ADM Computer Services Ltd (The Online Teacher)");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 52,
                column: "Name",
                value: "Simply Learning Tuition Agency Ltd");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 53,
                column: "Name",
                value: "Prospero Group Ltd");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 54,
                column: "Name",
                value: "Third Space Learning/Virtual Class Ltd");

            migrationBuilder.UpdateData(
                table: "TuitionPartners",
                keyColumn: "Id",
                keyValue: 56,
                column: "Name",
                value: "Assess Education Ltd");
        }
    }
}
