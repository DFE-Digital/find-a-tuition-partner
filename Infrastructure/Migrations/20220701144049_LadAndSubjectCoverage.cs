using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class LadAndSubjectCoverage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalAuthorityDistrictCoverage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TuitionPartnerId = table.Column<int>(type: "integer", nullable: false),
                    TuitionTypeId = table.Column<int>(type: "integer", nullable: false),
                    LocalAuthorityDistrictId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalAuthorityDistrictCoverage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalAuthorityDistrictCoverage_LocalAuthorityDistricts_Loca~",
                        column: x => x.LocalAuthorityDistrictId,
                        principalTable: "LocalAuthorityDistricts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocalAuthorityDistrictCoverage_TuitionPartners_TuitionPartn~",
                        column: x => x.TuitionPartnerId,
                        principalTable: "TuitionPartners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocalAuthorityDistrictCoverage_TuitionTypes_TuitionTypeId",
                        column: x => x.TuitionTypeId,
                        principalTable: "TuitionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectCoverage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TuitionPartnerId = table.Column<int>(type: "integer", nullable: false),
                    TuitionTypeId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectCoverage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectCoverage_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectCoverage_TuitionPartners_TuitionPartnerId",
                        column: x => x.TuitionPartnerId,
                        principalTable: "TuitionPartners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectCoverage_TuitionTypes_TuitionTypeId",
                        column: x => x.TuitionTypeId,
                        principalTable: "TuitionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistrictCoverage_LocalAuthorityDistrictId",
                table: "LocalAuthorityDistrictCoverage",
                column: "LocalAuthorityDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionPartnerId",
                table: "LocalAuthorityDistrictCoverage",
                column: "TuitionPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAuthorityDistrictCoverage_TuitionTypeId",
                table: "LocalAuthorityDistrictCoverage",
                column: "TuitionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCoverage_SubjectId",
                table: "SubjectCoverage",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCoverage_TuitionPartnerId",
                table: "SubjectCoverage",
                column: "TuitionPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCoverage_TuitionTypeId",
                table: "SubjectCoverage",
                column: "TuitionTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalAuthorityDistrictCoverage");

            migrationBuilder.DropTable(
                name: "SubjectCoverage");
        }
    }
}
